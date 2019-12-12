# Trucks'n'Guns : Endless Road War

Code of my first and only large mobile project at the moment.

The principle of operation is based on a "single entry point", (almost) all dependencies are implemented through the "general game state handler" script, which also launches all game "handlers".
On the Internet, everyone swears at singletones, service locators, event systems, and so I did it because I don’t know yet how to do it better ...

From the patterns, I used a state machine to separate the types of enemy behavior and a scriptable object pool to separate the types of pools. 
From the algorithms, only the path search algorithm was used.
![alt text](https://github.com/KonstantKuz/TruckDefense/blob/master/scr/1.png?raw=true)
![alt text](https://github.com/KonstantKuz/TruckDefense/blob/master/scr/3.png?raw=true)
![alt text](https://github.com/KonstantKuz/TruckDefense/blob/master/scr/4.png?raw=true)
![alt text](https://github.com/KonstantKuz/TruckDefense/blob/master/scr/11.png?raw=true)
![alt text](https://github.com/KonstantKuz/TruckDefense/blob/master/scr/6.png?raw=true)
![alt text](https://github.com/KonstantKuz/TruckDefense/blob/master/scr/7.png?raw=true)
![alt text](https://github.com/KonstantKuz/TruckDefense/blob/master/scr/8.png?raw=true)
![alt text](https://github.com/KonstantKuz/TruckDefense/blob/master/scr/9.png?raw=true)
![alt text](https://github.com/KonstantKuz/TruckDefense/blob/master/scr/10.png?raw=true)
![alt text](https://github.com/KonstantKuz/TruckDefense/blob/master/scr/2.png?raw=true)