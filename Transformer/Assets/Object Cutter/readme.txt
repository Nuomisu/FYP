Hi, 
This is guid how to use cutter script.
Attach cutter.js on that object which will be cutter, add component rigidbody(uncheck useGravity checkbox)and in the inspector window check isTrigger chekbox.
There will be created tags: "primitive" and "splitted".
Tag "primitive"  is used to know how to manipulate mesh's vertices, if object hasn't that tag, it's vertices will be pushed in middle. Sometimes "primitive" tag isn't working fine, for example it gaves more good result if you won't add tag on hose object.
Tag "splitted" is used for multiple cutting option, if on cutter object you'll choose splitonce checkbox, after cutting the object's tag will be "splitted", any object taged with "splitted" won't be cutted.
If you'll check boxcollider checkbox on cutter object, it will destroy any collider and it'll add boxcollider after cutting, if you won't check that, objects will have meshcollider after cutting.

If you use this script on mobile phone select cutter object, remove move.js script and add touchMovement.js to it.

You can see how it is working here: http://forum.unity3d.com/threads/50022-object-splitter/page2

Best regards,
Teimuraz Kokiashvili