using System;
using System.Runtime.InteropServices;

namespace Gumbo
{
    [StructLayout(LayoutKind.Sequential)]
    public class GumboErrorContainer
    {
        /// <summary>
        /// The type of error.        
        /// </summary>
        public GumboErrorType type;
        /// <summary>
        /// The position within the source file where the error occurred.
        /// </summary>
        public GumboSourcePosition position;
        /// <summary>
        /// A pointer to the byte within the original source file text where the error
        /// occurred (note that this is not the same as position.offset, as that gives
        /// character-based instead of byte-based offsets).
        /// </summary>
        public IntPtr original_text;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class GumboCodepointErrorContainer : GumboErrorContainer
    {
        /// <summary>
        /// The code point we encountered, for:
        /// * GUMBO_ERR_UTF8_INVALID
        /// * GUMBO_ERR_UTF8_TRUNCATED
        /// * GUMBO_ERR_NUMERIC_CHAR_REF_WITHOUT_SEMICOLON
        /// * GUMBO_ERR_NUMERIC_CHAR_REF_INVALID
        /// </summary>
        public ulong codepoint;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class GumboTokenizerErrorContainer : GumboErrorContainer
    {
        /// <summary>
        /// Tokenizer errors.
        /// </summary>
        public GumboTokenizerError tokenizer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class GumboNamedCharErrorContainer : GumboErrorContainer
    {
        /// <summary>
        /// Short textual data, for:
        /// * GUMBO_ERR_NAMED_CHAR_REF_WITHOUT_SEMICOLON
        /// * GUMBO_ERR_NAMED_CHAR_REF_INVALID
        /// </summary>
        public GumboStringPiece text;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class GumboDuplicateAttrErrorContainer : GumboErrorContainer
    {
        /// <summary>
        /// Duplicate attribute data, for GUMBO_ERR_DUPLICATE_ATTR.
        /// </summary>
        public GumboDuplicateAttrError duplicate_attr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class GumboParserErrorContainer : GumboErrorContainer
    {
        /// <summary>
        /// Parser state, for GUMBO_ERR_PARSER and
        /// GUMBO_ERR_UNACKNOWLEDGE_SELF_CLOSING_TAG.
        /// </summary>
        public GumboParserError parser;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GumboTokenizerError
    {
        /// <summary>
        /// The bad codepoint encountered.
        /// </summary>
        public int codepoint;
        /// <summary>
        /// The state that the tokenizer was in at the time.
        /// </summary>
        public GumboTokenizerErrorState state;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GumboStringPiece
    {
        /// <summary>
        /// A pointer to the beginning of the string.  NULL iff length == 0.
        /// </summary>
        public IntPtr data;
        /// <summary>
        /// The length of the string fragment, in bytes.  May be zero.
        /// </summary>
        public uint length;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GumboDuplicateAttrError
    {
        /// <summary>
        /// The name of the attribute.  Owned by this struct.
        /// </summary>
        public IntPtr name;
        /// <summary>
        /// The (0-based) index within the attributes vector of the original
        /// occurrence.
        /// </summary>
        public uint original_index;
        /// <summary>
        /// The (0-based) index where the new occurrence would be.
        /// </summary>
        public uint new_index;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GumboParserError
    {
        /// <summary>
        /// The type of input token that resulted in this error.        
        /// </summary>
        public GumboTokenType input_type;
        /// <summary>
        /// The HTML tag of the input token.  TAG_UNKNOWN if this was not a tag token.        
        /// </summary>
        public GumboTag input_tag;
        /// <summary>
        /// The insertion mode that the parser was in at the time.        
        /// </summary>
        public GumboInsertionMode parser_state;
        /// <summary>
        /// The tag stack at the point of the error.  Note that this is an GumboVector
        /// of GumboTag's *stored by value* - cast the void* to an GumboTag directly to
        /// get at the tag.
        /// /// </summary>
        public GumboVector /* GumboTag */ tag_stack;
    }

    public enum GumboInsertionMode
    {
        GUMBO_INSERTION_MODE_INITIAL,
        GUMBO_INSERTION_MODE_BEFORE_HTML,
        GUMBO_INSERTION_MODE_BEFORE_HEAD,
        GUMBO_INSERTION_MODE_IN_HEAD,
        GUMBO_INSERTION_MODE_IN_HEAD_NOSCRIPT,
        GUMBO_INSERTION_MODE_AFTER_HEAD,
        GUMBO_INSERTION_MODE_IN_BODY,
        GUMBO_INSERTION_MODE_TEXT,
        GUMBO_INSERTION_MODE_IN_TABLE,
        GUMBO_INSERTION_MODE_IN_TABLE_TEXT,
        GUMBO_INSERTION_MODE_IN_CAPTION,
        GUMBO_INSERTION_MODE_IN_COLUMN_GROUP,
        GUMBO_INSERTION_MODE_IN_TABLE_BODY,
        GUMBO_INSERTION_MODE_IN_ROW,
        GUMBO_INSERTION_MODE_IN_CELL,
        GUMBO_INSERTION_MODE_IN_SELECT,
        GUMBO_INSERTION_MODE_IN_SELECT_IN_TABLE,
        GUMBO_INSERTION_MODE_AFTER_BODY,
        GUMBO_INSERTION_MODE_IN_FRAMESET,
        GUMBO_INSERTION_MODE_AFTER_FRAMESET,
        GUMBO_INSERTION_MODE_AFTER_AFTER_BODY,
        GUMBO_INSERTION_MODE_AFTER_AFTER_FRAMESET,
    }

    public enum GumboTokenType
    {
        GUMBO_TOKEN_DOCTYPE,
        GUMBO_TOKEN_START_TAG,
        GUMBO_TOKEN_END_TAG,
        GUMBO_TOKEN_COMMENT,
        GUMBO_TOKEN_WHITESPACE,
        GUMBO_TOKEN_CHARACTER,
        GUMBO_TOKEN_NULL,
        GUMBO_TOKEN_EOF,
    }

    public enum GumboErrorType
    {
        GUMBO_ERR_UTF8_INVALID,
        GUMBO_ERR_UTF8_TRUNCATED,
        GUMBO_ERR_UTF8_NULL,
        GUMBO_ERR_NUMERIC_CHAR_REF_NO_DIGITS,
        GUMBO_ERR_NUMERIC_CHAR_REF_WITHOUT_SEMICOLON,
        GUMBO_ERR_NUMERIC_CHAR_REF_INVALID,
        GUMBO_ERR_NAMED_CHAR_REF_WITHOUT_SEMICOLON,
        GUMBO_ERR_NAMED_CHAR_REF_INVALID,
        GUMBO_ERR_TAG_STARTS_WITH_QUESTION,
        GUMBO_ERR_TAG_EOF,
        GUMBO_ERR_TAG_INVALID,
        GUMBO_ERR_CLOSE_TAG_EMPTY,
        GUMBO_ERR_CLOSE_TAG_EOF,
        GUMBO_ERR_CLOSE_TAG_INVALID,
        GUMBO_ERR_SCRIPT_EOF,
        GUMBO_ERR_ATTR_NAME_EOF,
        GUMBO_ERR_ATTR_NAME_INVALID,
        GUMBO_ERR_ATTR_DOUBLE_QUOTE_EOF,
        GUMBO_ERR_ATTR_SINGLE_QUOTE_EOF,
        GUMBO_ERR_ATTR_UNQUOTED_EOF,
        GUMBO_ERR_ATTR_UNQUOTED_RIGHT_BRACKET,
        GUMBO_ERR_ATTR_UNQUOTED_EQUALS,
        GUMBO_ERR_ATTR_AFTER_EOF,
        GUMBO_ERR_ATTR_AFTER_INVALID,
        GUMBO_ERR_DUPLICATE_ATTR,
        GUMBO_ERR_SOLIDUS_EOF,
        GUMBO_ERR_SOLIDUS_INVALID,
        GUMBO_ERR_DASHES_OR_DOCTYPE,
        GUMBO_ERR_COMMENT_EOF,
        GUMBO_ERR_COMMENT_INVALID,
        GUMBO_ERR_COMMENT_BANG_AFTER_DOUBLE_DASH,
        GUMBO_ERR_COMMENT_DASH_AFTER_DOUBLE_DASH,
        GUMBO_ERR_COMMENT_SPACE_AFTER_DOUBLE_DASH,
        GUMBO_ERR_COMMENT_END_BANG_EOF,
        GUMBO_ERR_DOCTYPE_EOF,
        GUMBO_ERR_DOCTYPE_INVALID,
        GUMBO_ERR_DOCTYPE_SPACE,
        GUMBO_ERR_DOCTYPE_RIGHT_BRACKET,
        GUMBO_ERR_DOCTYPE_SPACE_OR_RIGHT_BRACKET,
        GUMBO_ERR_DOCTYPE_END,
        GUMBO_ERR_PARSER,
        GUMBO_ERR_UNACKNOWLEDGED_SELF_CLOSING_TAG,
    }

    public enum GumboTokenizerErrorState
    {
        GUMBO_ERR_TOKENIZER_DATA,
        GUMBO_ERR_TOKENIZER_CHAR_REF,
        GUMBO_ERR_TOKENIZER_RCDATA,
        GUMBO_ERR_TOKENIZER_RAWTEXT,
        GUMBO_ERR_TOKENIZER_PLAINTEXT,
        GUMBO_ERR_TOKENIZER_SCRIPT,
        GUMBO_ERR_TOKENIZER_TAG,
        GUMBO_ERR_TOKENIZER_SELF_CLOSING_TAG,
        GUMBO_ERR_TOKENIZER_ATTR_NAME,
        GUMBO_ERR_TOKENIZER_ATTR_VALUE,
        GUMBO_ERR_TOKENIZER_MARKUP_DECLARATION,
        GUMBO_ERR_TOKENIZER_COMMENT,
        GUMBO_ERR_TOKENIZER_DOCTYPE,
        GUMBO_ERR_TOKENIZER_CDATA,
    }
}
