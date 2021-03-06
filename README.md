# Playing Snake w/ Webcam-Based Controls
Using ML.NET with Unity for gesture recognition to play the classic game, Snake.

## Concept
This version of the classic game, Snake, uses the C# ML.NET library to train vision-based models for different directions (UP, DOWN, LEFT, RIGHT). This model is then used in Unity with a webcam to translate hand positions into controls for the snake's movements.

The project is composed of two pieces, the trainer and the game. 

## Trainer
The trainer is a C# console app that uses 5 different folders (UP, DOWN, LEFT, RIGHT, UNKNOWN) of various images to train a classification model.

![Folders w/ training images](https://raw.githubusercontent.com/kevin-r-jordan/snake-ml-vision/main/docs/training-images.png "Folders with training images")

The image classifier uses [ResNet v2 101](https://tfhub.dev/google/imagenet/resnet_v2_101/classification/4) with 50 epochs. As this was a proof of concept project, no attempt was made to improve the effectiveness of the model.

![Console windows after trainer execution](https://raw.githubusercontent.com/kevin-r-jordan/snake-ml-vision/main/docs/trainer-output.png "Console windows after trainer execution")

The output of the trainer is a model.zip file for use in the Unity game.

## Snake Game
The Snake game was created using the [Unity Snake Tutorial](https://github.com/zigurous/unity-snake-tutorial) provided by Adam Graham.

![Snake game screenshot](https://raw.githubusercontent.com/kevin-r-jordan/snake-ml-vision/main/docs/game-screenshot.png "Snake game screenshot")

When the game is loaded, the model.zip is loaded and a singleton of the prediction engine is available for the other game objects.

A webcam capture script continuously generates png images from the user's webcam. An option to save these images to the local file system is also available for future training data. As each image is generated, the prediction engine is called with the png byte data.

The prediction engine raises an event each time a new prediction is generated. The snake script subscribes to this event and determines if the snake's direction should change.

Incorporating ML.NET into Unity was not straightforward, but through trial and error of different combination of libraries, the necessary dependencies were added into the Plugins directory.
