# About

WIP.

Simple FPP controller with interaction system. My goal is to:

1. Make it immersive by requiring player to simulate interaction with his mouse i.e actually spin it when spinning valve, pull it when pulling lever.
2. Make it expandable and adjustable, so I can use it in my future projects.

https://user-images.githubusercontent.com/80332947/212591350-630ac35a-50e1-4e95-a025-d24707ab00fe.mp4


https://user-images.githubusercontent.com/80332947/212591362-69a0eee2-31fa-43de-8d05-d106b94e2697.mp4


https://user-images.githubusercontent.com/80332947/212591365-e7e58ede-9837-416d-a4f8-e509974049db.mp4


https://user-images.githubusercontent.com/80332947/212591986-349307cc-b1e3-40a2-aad0-d384ad0bd77a.mp4

# Links

* [Scripts Folder](https://github.com/Ys95/InteractionsDemo/tree/main/Assets/PuzzleDungeon/Scripts)
* [Itch.io (Playable in browser)](https://ys95.itch.io/dungeon-demo)

## Quick description of most important classes:
* [InputManager](https://github.com/Ys95/InteractionsDemo/blob/main/Assets/PuzzleDungeon/Scripts/Input/InputManager.cs) - Based on new input system, responsible for processing input so it can be used by other scripts.
* [CharacterCompontent](https://github.com/Ys95/InteractionsDemo/blob/main/Assets/PuzzleDungeon/Scripts/Character/CharacterComponent.cs) - Abstract class, base for various player functionalities and abilities like moving or interacting.
* [CharacterHub](https://github.com/Ys95/InteractionsDemo/blob/main/Assets/PuzzleDungeon/Scripts/Character/CharacterHub.cs) - Manages character components. 
* [CharacterMovement](https://github.com/Ys95/InteractionsDemo/blob/main/Assets/PuzzleDungeon/Scripts/Character/CharacterMovement.cs) - Component that allows player to move. Based on CharacterController, with simple movement physics simulation like counter movement when changing direction or gradual velocity loss when player stops moving.
* [CharacterInteraction](https://github.com/Ys95/InteractionsDemo/blob/main/Assets/PuzzleDungeon/Scripts/Character/CharacterInteractions.cs) - Allows to interact with environment and search for interactable objects, manages "anchor" which is like a player hand that is used to move objects etc.
* [Interactable](https://github.com/Ys95/InteractionsDemo/blob/main/Assets/PuzzleDungeon/Scripts/Interactions/Interactable.cs) - Base class for interactable objects.
* [GrabbableObject](https://github.com/Ys95/InteractionsDemo/blob/main/Assets/PuzzleDungeon/Scripts/Interactions/GrabbableObject.cs) - Object that can be grabbed or moved.

