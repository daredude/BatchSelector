# fj.BatchSelector

_Sometimes you just want a UI to select a script._

### Installation

You can copy the fj.BatchSelector.exe where ever you want and execute it directly.
The application requires a Files.xml file with selectable items to render.

### Files.xml

The root node is supposed to be named as `FileList`. It has the attributes `Title` (will be shown as window title) and `LoaderMessage` (executing script message).

The root node will contain elements with the name `File`. These elements should have a `Label` attribute which will be rendered as button label and `FileName` attribute which can be an absolute or relative path to a batch script.