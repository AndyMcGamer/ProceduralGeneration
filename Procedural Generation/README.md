# Townscaper-Like Organic Grids with N-Gons
I've always been fascinated by Oskar Stålberg's amazing work [Townscaper](https://store.steampowered.com/app/1291340/Townscaper/), and I wanted to try reproducing those beautiful organic grids. Luckily, Oskar revealed how he went about creating his grids in [this wonderful talk](https://www.youtube.com/watch?v=1hqt8JkYRdI).
In the talk, he shows the process of creating one chunk of this organic grid:
1. Start with a locus of equidistant points forming a regular hexagon
2. Triangulate the hexagon into equilateral triangles
3. Join some of the adjacent triangles together to form quadrilaterals (not all will be turned into quads)
4. Subdivide the quads and triangles into 4 smaller quads
5. Try to "relax" the vertices of these quads and make each one more "square"

Now, Oskar uses hexagons as they are very symmetrical and work out really well with the triangles all being equilateral and whatnot
But I wondered what it would look like for other polygons like pentagons or squares or heptagons or even dodecagons and so I sought out to create these organic grids with these polygons and compare with hexagons.

## Method for Generation
The process I used to create these grids was slightly different from Oskar's:
1. Create a locus of points in the shape of the n-gon. 
   * To accomplish this, I looped r times (r is the radius) and created larger and larger polygons [like how it's shown here](https://twitter.com/osksta/status/1147881669350891521)

1. Then, I implemented the [Bowyer-Watson Algorithm for Delaunay Triangulation](https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm) to triangulate the n-gon
2. Join at random some of the triangles into quadrilaterals
3. Subdivide the quads and triangles into 4 smaller quads
4. Relax the vertices of the quads to reveal the final result
   * Now for the actual relaxation algorithm, I decided to take a different approach from Oskar, who used an iterative approach
   * Instead, I took reference from [this really neat article](https://andersource.dev/2020/11/06/organic-grid.html) by [Daniel Anderson](https://andersource.dev/) which uses a constrait-based calculus approach that makes a lot more sense to me.
   * In essence, we define inside each quad a set of vertices that make a square with a predefined side length. Let's call them "square vertices"
   * Basically, we derive an equation such that we are able to calculate the exact coordinates of these square vertices, and we can then move the quad's actual vertices to become those square vertices using a bit of vector math. Let's refer to the vector difference between the square vertex and its respective actual vertex as a "force"
   * Now, each vertex is part of 2-4 different quads, so instead of choosing one to move to, we take the average of the forces applied on each vertex to get it's actual final position
   * I found this method of relaxing the vertices to be much more straightforward and intuitive than Oskar's version, and it yields pretty good results.

## Now that we can generate an organic grid from any n-gon shape, what are the results?
Unsuprisingly, the most visually pleasing grids were the ones where I start with a hexagon. Hexagons are just the perfect shape for creating these organic grids in this manner because of those equilateral triangles.
But, there were still some close contenders. I was happy to find that pentagons, heptagons, octagons and even nonagons yielded pleasurable results, and honestly there wasn't that big of a difference between those polygons and the hexagon.
Anything less than pentagon or more than 10 sides seems to be too skewed for it to work feasibly as a reliable grid system, though perhaps you could perform the relaxation multiple times and maybe it'll improve.

## Conclusions
Overall, I had a lot of fun playing around with all these different shapes. I think there are definitely shapes that are 100% viable for creating organic grids and in some cases I think it might even yield more interesting shapes than the simple hexagon. 
