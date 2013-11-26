Compare .Net Objects Easily
======================

The purpose of this library is to all the comparison of 2 different .Net objects based on properties with similar names.

This library library does not require that the objects are of the same type as it uses reflection to get property values.

It will examine all properties with a read accessor even if the accessor is marked private. However it does allow you to skip properties you are not interested in testing.

Road-map
--------
[x] Upload base C# implementation with tests.<br>
[x] Fix the object comperer to not reley on generic properties.<br>
[x] Create an F# project to allow easier addition of features<br>
[x] Create an object to better represent the results of comparing properties.<br>
<br>
[ ] Move features to F#<br>
[ ] Create an object to better represent the properties, and their values, of an object.<br>

> Written with [StackEdit](https://stackedit.io/).