# _Virtual Parallel robots. Forward and Inverse Kinematics_

- The main topic of this project outlines the development of two parallel robots in a virtual environment: the Gough-Stewart platform and the Delta Robot.
- An important branch of the vast field of robotics is the analysis of the parallel robots from a kinematic point of view. 
- The focus is on the structure and representation of the robots mentioned above, their forward and inverse kinematics, and, last but not least, on the integration of kinematic algorithms in real-world uses through various virtual simulations.

The methods to determine inverse kinematics for parallel manipulators are easy to apply. On the other hand, forward kinematics is a much more complex problem due to the lack of information on the actuated arms. The forward kinematics of the Stewart platform proposes the iterative numerical Newton-Raphson algorithm explained in detail with mathematical equations. In the case of Delta Robot, a different solving method is used since the end effector performs only translation movements on the three coordinate axes: the method of the intersection of three invisible spheres is approached.

The virtual world takes shape with the integration of the forward and inverse kinematics in different utilization. The personal contributions are presented in the project due to the decisions and choices made, the obtained results in the application of algorithms, but also due to the final representations in the virtual environment. Therefore, the application proposes various simulations where the user can interact with the robots by changing the parameters of the joints or the end-effector position and rotation.

### Tech

- Unity: used for virtual simulations
- Blender: used for 3D modeling of robots
- C#
- Math.NET Numerics Framework

### Simulations

#### *Gough Stewart Platform*

- Inverse Kinematics: vectorial method
    - ![alt text](https://github.com/robuvlad/IK-Unity/blob/master/Assets/Thesis_Images/SP_Update_3.PNG)
    - ![alt text](https://github.com/robuvlad/IK-Unity/blob/master/Assets/Thesis_Images/SPStructure1.png)

- Forward Kinematics: Newthon - Raphson Method
    - ![alt text](https://github.com/robuvlad/IK-Unity/blob/master/Assets/Thesis_Images/SP_Update_4.PNG)
    
#### *Delta Robot*

- Inverse Kinematics: analitical method
    - ![alt text](https://github.com/robuvlad/IK-Unity/blob/master/Assets/Thesis_Images/DR_Update_1.PNG)
    - ![alt text](https://github.com/robuvlad/IK-Unity/blob/master/Assets/Thesis_Images/DeltaRobotStructure.png)

- Forward Kinematics: intersecting the spheres method
    - ![alt text](https://github.com/robuvlad/IK-Unity/blob/master/Assets/Thesis_Images/DR_Update_2.PNG)
    - ![alt text](https://github.com/robuvlad/IK-Unity/blob/master/Assets/Thesis_Images/DeltaRobotStructureSphere.png)