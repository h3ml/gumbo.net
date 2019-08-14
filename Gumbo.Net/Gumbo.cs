using Gumbo.Xml;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Gumbo
{
    public struct GumboLibraryOptions
    {
        public bool StopOnFirstError { get; set; }
        public int MaxErrors { get; set; }
        public int TabStopSize { get; set; }
        public GumboTag FragmentContext { get; set; }
        public GumboNamespaceEnum FragmentNamespace { get; set; }
    }

    public sealed class Gumbo : IDisposable, IXPathNavigable
    {
        public Document Document { get; }
        public IEnumerable<GumboErrorContainer> Errors { get; }
        bool _disposed;
        bool _marshalled;
        GumboOptions _options;
        readonly GumboDocumentNode _gumboDocumentNode;
        readonly IntPtr _outputPtr;
        readonly IntPtr _html;
        readonly GumboFactory _gumboFactory;
        readonly INativeLibrary _gumboLibrary = NativeLibrary.Create(NativeMethods.LibraryName);

        public Gumbo(string html, GumboLibraryOptions? options = null)
        {
            _options = CreateOptions(options);
            _html = NativeUtf8.NativeUtf8FromString(html);
            _outputPtr = NativeMethods.gumbo_parse(_html);
            var output = Marshal.PtrToStructure<GumboOutput>(_outputPtr);
            _gumboDocumentNode = output.GetDocument();
            Errors = output.GetErrors();
            var lazyFactory = new LazyFactory(() => _disposed, typeof(Gumbo).Name);
            _gumboFactory = new GumboFactory(lazyFactory);
            Document = (Document)_gumboFactory.CreateNode(_gumboDocumentNode);
        }
        ~Gumbo() { Dispose(); }

        GumboOptions CreateOptions(GumboLibraryOptions? options)
        {
            var defaultOptions = _gumboLibrary.MarshalStructure<GumboOptions>("kGumboDefaultOptions");
            if (options != null)
            {
                defaultOptions.max_errors = options.Value.MaxErrors;
                defaultOptions.stop_on_first_error = options.Value.StopOnFirstError;
                defaultOptions.tab_stop = options.Value.TabStopSize;
                defaultOptions.fragment_context = options.Value.FragmentContext;
                defaultOptions.fragment_namespace = options.Value.FragmentNamespace;
            }
            return defaultOptions;
        }

        public XDocument ToXDocument() => GumboExtensions.ToXDocument(_gumboDocumentNode);

        public XPathNavigator CreateNavigator() => new GumboNavigator(this, Document);

        public Element GetElementById(string id)
        {
            MarshalAll();
            return _gumboFactory.GetElementById(id);
        }

        /// <summary>
        /// Disposes all unmanaged data. Any subsequent calls to get nodes' children
        /// not previously marshalled will result in exception.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;
            Marshal.FreeHGlobal(_html);
            NativeMethods.gumbo_destroy_output(ref _options, _outputPtr);
            _gumboLibrary.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// Marshals all nodes. Does nothing if has already been called.
        /// </summary>
        public void MarshalAll()
        {
            if (_marshalled)
                return;
            MarshalElementAndDescendants(Document.Root);
            _marshalled = true;
        }

        static void MarshalElementAndDescendants(Element element)
        {
            GC.KeepAlive(element.Attributes);
            foreach (var child in element.Children.OfType<Element>())
                MarshalElementAndDescendants(child);
        }
    }
}
