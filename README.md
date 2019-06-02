# I-AM-HALO / Unity

Unity project for I AM HALO VR experience. 

Project Unity version = 2018.1.6f1

Assets/Scenes:
- PerformanceVR: scene to be projected at live performances
- Forest Light Temple: music reactive tilt brush scene with unicorn
- Infinite Zoom: Use arrow keys to scale the world from Space to Earth to the Forest! (still quite buggy)
- Mesh Deformation: Meshes are deformed ambiently, on click (click and drag on blue sphere), and on collision
- Unicorn: A unicorn in a forest, can be controlled with WASDER, space, and arrow keys
- Vive Test: vive rig with pink fire particles coming from the controllers!
- Drawing3D: draw lines in 3D with WASD keys

--------------------------------------

TO USE SPOUT (frame buffers for live projection), open PerformaceVR scene and enable the "Spout-FrameBuffers" object.

--------------------------------------

TO CREATE A NEW PATH in the PerformanceVR scene: 

- in the Hierarchy, select Pivots > MainStage > PathToFlowerBridge
- CTRL+C copy and CTRL+V paste
- drag the copy onto Pivots > [the pivot you want it to start at] so that it nests under it
- rename it PathTo[destination pivot]
- expand Pivots > [starting pivot] > PathTo[destination pivot]
- drag Point onto Pivots > [starting pivot]
- in the Inspector, right click Transform and choose Reset, now the position and rotation should be all zeros and the scale all ones.
- drag Point back onto PathTo[destination pivot]. now the first point of the path is aligned to the starting pivot
- repeat for the last point, Point(3), except align it to the destination pivot. drag it onto Pivots > [destination pivot], reset the transform, and drag it back onto PathTo[destination pivot]
- now select Point(1)
- in the Scene, you should see a 3D cursor with 3 arrows, if not press W or click the 4 headed arrow button in the top left (next to the hand button)
- click and drag the 3D cursor and notice the curve in the scene updates. you'll want to rotate around the scene (ALT+drag) to make sure the line is going where you want in 3D
- do this for Point(2) as well until the line goes where you like
- select Pivots > [starting pivot]. in the Inspector, under the Pivot component, increase the size of Paths by 1. A new item should appear at the end of the list
- on the new item, for Destination Name, type the name of [starting pivot] (unfortunately this needs to exactly match the name of the object in the Hierarchy for now)
- drag Pivots > [starting pivot] > PathTo[destination pivot] into the slot for Spline to Next Pivot 
- for World Boundary Percent On Spline, X is the percent on the path when the geometry for the next world will appear, Y is the percent on the path when the geometry for the starting world will disappear. you'll need to test this in play mode to get the right values, but you can start with 40, 60
- speed is how fast the camera travels the path, this needs to be >0, 20 is plenty. Once again, will need to test and adjust
- select ObserverPivot in the Hierarchy
- in the Inspector, under the Pivot Switcher component, increase the size of Pivots by 1. drag the Pivots > [destination pivot] into the last slot in the list
- click play and test the new path (press up arrow to go to next pivot). you'll probably need to tweak these settings once you try it (especially the positions of Point(1) and Point(2), the speed, and the World Boundary percentages)



