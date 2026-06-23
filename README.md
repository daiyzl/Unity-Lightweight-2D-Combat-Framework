# Unity Lightweight 2D Combat Framework

A clean, modular and highly decoupled 2D action game framework built with Unity. 
Relatively simple and lightweight, this project only focuses on core combat logic, code-driven combat juice, and scalable AI architecture

## 🌟 Core Features

* **Modular FSM AI (有限状态机):** A fully decoupled Finite State Machine for enemy behavior (Idle, Patrol, Chase, Attack). Implements the Blackboard pattern (`Parameter` class) to separate data from state logic, ensuring zero-coupling when adding new monsters or skills.
* **Global Combat "Juice" (全局打击感反馈):** An independent Singleton `JuiceManager` handling real-time Hit Stop (time scale manipulation) and Camera Shake. Ensures visceral combat feedback without polluting entity scripts.
* **Decoupled Hit Detection (独立伤害结算):** Integrate the FSM state machine to dynamically control the damage values of attack colliders in real time. Through an in-depth analysis of Unity coroutine lifecycles, real-time unscaled time sequences are adopted to independently handle enemy hit flash effects. This completely resolves timeline deadlocks caused by global hit-stop slowdowns and achieves thorough decoupling of game logic and visual presentation.
* **Custom Kinematic Physics (纯代码物理边界防穿模):** Bypasses standard Unity physics bouncing by utilizing custom Raycast collision detection. Ensures pixel-perfect dash and smash attacks without wall-clipping or terrain glitches.

## 🛠️ Tech Stack
* **Engine:** Unity 2022+ (2D Pipeline)
* **Language:** C#
* **Architecture:** Finite State Machine (FSM), Singleton Pattern, Blackboard Pattern, Component-Based Design.

## 🚀 Getting Started
Clone the repository and open the project in Unity. Explore the `Scripts/` folder to view the decoupled architecture, specifically `FSM.cs` and the `IState` implementations.
