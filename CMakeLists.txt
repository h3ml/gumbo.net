cmake_minimum_required(VERSION 3.8.0)
project(gumbo)

# WINDOWS
if(WIN32)
   set(VCPKG ${CMAKE_SOURCE_DIR}/_vcpkg)

   # install vcpkg
   if (NOT IS_DIRECTORY ${VCPKG})
      message("vcpkg does not exist. installing...")
      execute_process(
         COMMAND git.exe clone https://github.com/Microsoft/vcpkg.git ${VCPKG}
         WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
      )
      execute_process(
         COMMAND bootstrap-vcpkg.bat
         WORKING_DIRECTORY ${VCPKG}
      )
   endif()

   # install gumbo:x86-windows
   if (NOT IS_DIRECTORY ${VCPKG}/packages/gumbo_x86-windows)
      execute_process(
         COMMAND vcpkg install gumbo:x86-windows
         WORKING_DIRECTORY ${VCPKG}
      )
   endif()
   # generate gumbo.def
   if (NOT EXISTS ${VCPKG}/packages/gumbo_x86-windows/lib/gumbo.def)
      message("x86 gumbo.def. generating...")
      execute_process(
         COMMAND dotnet run --project ./Gumbo.DefGen x86 ${VCPKG}/packages/gumbo_x86-windows/lib/gumbo.lib
         WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
      )
   endif()

   # install gumbo:x64-windows
   if (NOT IS_DIRECTORY ${VCPKG}/packages/gumbo_x64-windows)
      execute_process(
         COMMAND vcpkg install gumbo:x64-windows
         WORKING_DIRECTORY ${VCPKG}
      )
   endif()
   # generate gumbo.def
   if (NOT EXISTS ${VCPKG}/packages/gumbo_x64-windows/lib/gumbo.def)
      message("x64 gumbo.def. generating...")
      execute_process(
         COMMAND dotnet run --project ./Gumbo.DefGen x64 ${VCPKG}/packages/gumbo_x64-windows/lib/gumbo.lib
         WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}
      )
   endif()
endif()

add_library(gumbo SHARED empty.cc ${VCPKG}/packages/gumbo_${BITNESS}-windows/lib/gumbo.def)
target_link_libraries(gumbo ${VCPKG}/packages/gumbo_${BITNESS}-windows/lib/gumbo.lib)