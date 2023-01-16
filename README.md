![cover2]()

# About

Simple FPP controller with interaction system. My goal was to:

1. Make it immersive by requiring player to simulate interaction with his mouse i.e actually spin it when spinning valve, pull it when pulling lever, reduce mouse sensitivity to simualte moving heavy object. 
2. Make it expandable and adjustable, so I can use it in my future projects.

It's more of a code sample than a game. 

# Links

* [Scripts Folder](https://github.com/Ys95/InteractionsDemo/tree/main/Assets/PuzzleDungeon/Scripts)
* [Itch.io (Playable in browser)](https://ys95.itch.io/dungeon-demo)

## Quick destricption of most important classes:
* [InputManager](https://github.com/Ys95/InteractionsDemo/blob/main/Assets/PuzzleDungeon/Scripts/Input/InputManager.cs) - Based on new input system, responsible for processing input so it can be used by other scripts.
* [CharacterCompontent](https://github.com/Ys95/InteractionsDemo/blob/main/Assets/PuzzleDungeon/Scripts/Character/CharacterComponent.cs) - Abstract class, base for various player functionalities and abiliteis, like moving or interacting. Whole concept was inspired by TopDown Engine.
* [CharacterHub](https://github.com/Ys95/InteractionsDemo/blob/main/Assets/PuzzleDungeon/Scripts/Character/CharacterHub.cs) - Manages character components. 
* [CharacterMovement](https://github.com/Ys95/InteractionsDemo/blob/main/Assets/PuzzleDungeon/Scripts/Character/CharacterMovement.cs) - Component that allows player to move. Based on CharacterController, with simple movement physics simulation like counter movement when changing direction or gradual velocity loss when player stops moving.
* [CharacterInteraction](https://github.com/Ys95/InteractionsDemo/blob/main/Assets/PuzzleDungeon/Scripts/Character/CharacterInteractions.cs) - Allows to interact with environment and search for interactable objects, manages "anchor" which is like a player hand that is used to move objects etc.
* [Interactable](https://github.com/Ys95/InteractionsDemo/blob/main/Assets/PuzzleDungeon/Scripts/Interactions/Interactable.cs) - Base class for interactable objects.
* [GrabbableObject](https://github.com/Ys95/InteractionsDemo/blob/main/Assets/PuzzleDungeon/Scripts/Interactions/GrabbableObject.cs) - Object that can be grabbed or moved.

