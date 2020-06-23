# Unity-Procedural-Landscape-Generation

An experiment into procedural generation to generate landmasses and landscapes using Perlin noise. This project features a relatively sophisticated system for creating and customising procedurally generated terrain. With different chunk sizes, the use of a fall-off map for generating islands and the ability to create different "biomes", with customisable `[AssetMenu()]` components. 

The chunks generated all have different LOD meshes, generated depending on their distance from the viewer. There are 6 different levels of details in the default chunk size (240) as 240 is evenly divisible by every integer up to 12. The chunks are generated across different threads, and collision detection is optimised. 

<img src="http://www.tomdotscott.com/images/Github/TerrainGeneration/LOD-Meshes.gif">

# Configurable Data

## Noise Data

Within the NoiseData assets, various factors to do with how the heightmap is generated can be tweaked. All of these are viewable in the editor, and an explanation of what they do as well as a gif to go alongside is shown here:
* The Normalisation Mode is used to toggle between one chunk and endless generation. It normalises all heights that are being generated, giving a coherent flow to the terrain that is generated, as well as tidying up the seams that get created between the chunks whether the fall-off map is applied or not. 
* The Noise Scale value changes the overall scale of the perlin noise, and thus changes how frequently values differ from each other across a distance. This is universal for the data; it applies across every octave of noise. 
* The Octaves value determines how many layers of noise there should be in each chunk. This allows for the meshes to have a more natural look. 
* The Persistence slider determines how quickly the amplitudes diminish for each successive octave in a Perlin-noise function. The amplitude of each successive octave is equal to the product of the previous octave's amplitude and the persistence value.
* A multiplier that determines how quickly the frequency increases for each successive octave in a Perlin-noise function. The frequency of each successive octave is equal to the product of the previous octave's frequency and the lacunarity value.
* Seed. This is the seed for the PNRG.
* Offset. Offsets the origin of the noise generated. 

<img src="http://www.tomdotscott.com/images/Github/TerrainGeneration/Noise-Data.gif">

## Terrain Data

Within the TerrainData assets, various factors can be configured that will change the look of the resulting Mesh.
* Uniform Scale. The Uniform Scale scales the entire mesh to the in-game unit size.
* Flat Shading. This toggle determines whether flat shading will be used when the mesh is generated. This uses a lower-poly mesh and changes the normals being generated to suit this style. 
* Fall-Off Map. This toggle determines whether the fall-off map should be applied to the noise map. The falloff map determines the point at which the values turn to 0. When toggled, the generator generates islands. 
* Height Multiplier. This slider determines the rate at which the heights change based on the value from the height map. 
* Mesh Height Curve. This allows for the blending of different levels, as the increase is mapped to a curve. This allows for a much more natural look to the generated terrain. 

<img src="http://www.tomdotscott.com/images/Github/TerrainGeneration/Terrain-Data.gif">

## Texture Data

The texture data assets deal with the application of colour and textures to the meshes using a HLSL shader. The editable features in this asset are the layers themselves, which each contain several features. 
* Texture. This container holds the actual texture to be displayed on the mesh.
* Tint. This is the tint that should be applied to the mesh. If there is any transparency in the textures, the tint can be used to cover it up. 
* Tint Strength. This slider is used to determine how visible the textures are compared to the tint. 
* Start Height. The value from this slider is used to determine the value on the heightmap where the texture should appear. 
* Blend Strength. This slider determines the amount that each texture gets blended together to ease transitions between the textures. 
* Texture Scale. This value determines how much the texture should be scaled up by when applied to the mesh. A higher value means that the tiling isn’t as noticeable, but means that the texture will be a lower resolution when applied. 

<img src="http://www.tomdotscott.com/images/Github/TerrainGeneration/Texture-Data.gif">

# Flying Through

Here are some examples of what the generation looks like. 

## With Fall-Off Map

<img src="http://http://www.tomdotscott.com/images/Github/TerrainGeneration/Flying-Fall-Off-Map.gif"> 

## Without Fall Off Map

<img src="http://www.tomdotscott.com/images/Github/TerrainGeneration/Flying.gif">

# Conclusion

This project has taught me a lot about Unity, through creating editable, updatable, meshes that are viewable in the editor, to making widgets and custom assets, I have developed my skills within this engine. I have also developed my shader skills, through developing a custom shader that loads in and blends the textures. On top of this, I have improved my overall programming skills in this project as the terrain is generated across multiple threads. 

## Features to add

In terms of features that I would like to add to this project in the coming weeks and months, I would like to add a proper water shader. On the whole, it will make the terrain generated feel more alive and I feel is not too far out of my current programming reach. 

I would also like to add procedural volumetric cloud generation to this project, inspired by <a href="https://github.com/SebLague">Sebastian Lague</a> in his <a href="https://www.youtube.com/watch?v=4QOcCGI6xOU">coding adventures video</a>.
