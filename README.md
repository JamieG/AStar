![alt a* c#](https://i.imgur.com/HBKpInx.png)
## High performance A* in C#

This is a base repo I use to benchmark and improve the pathing algorithm used in various projects, with the visuals it outputs it could be used as an educational tool when 
learning pathing, with an implementation that is performant enough to be viable in a production environment. 

The code is optimised for raw speed, in that the aim is speed at the expense of memory usage. If you can see a way to improve please let me know. Currently the bottleneck of the algorithm, at least based on DotTrace profiling is the priority queue enqueue which is already close to optimal for a managed memory solution. The aggregate effect of the heuristic algorithm is also a balance between performance and accuracy, as most of my projects allow for 8 directions of movement on a grid I went with octal distance. Heuristic bias could be tweaked based on the use case when accuracy is not as important.

## Benchmark (100 Runs)

The current benchmark maze results in 47771 cells in the closed list, with a path length of 1210, that gives an average step duration of 0.0112233ms and an average solve time of 14ms on an AMD Ryzen 7 1700 @ 2.99 Ghz.

![alt path result](https://i.imgur.com/Fi2cMCR.jpg)

## Output
![alt path result](https://i.imgur.com/ZFOgRyf.png)
