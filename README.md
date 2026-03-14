# RhythmKeyBot

RhythmKeyBot is a simple rhythm game bot that detects specific colors under the mouse cursor and simulates key/mouse input accordingly. It is designed to automate actions in `My Dystopian Robot Girlfriend` by responding to color changes on the screen.

## Features

- **Color Detection**: Detects specific colors under the mouse cursor.
- **Input Simulation**: Simulates mouse and keyboard inputs based on detected colors.
  - Holds the left mouse button ("Z" action) when blue or orange colors are detected.
  - Holds the "X" key (via scan code) when green or purple colors are detected.
- **Toggle Functionality**: Toggles the bot on/off using the `F8` key.
- **Console Feedback**: Continuously updates the status in the console with a spinner and debug information.

## How It Works

1. **Color Matching**:
   - The bot uses the Win32 API to read the pixel color under the mouse cursor.
   - Matches the detected color against predefined target colors:
     - Blue: `#2783F2`
     - Orange: `#FF6A29`
     - Green: `#009144`
     - Purple: `#502EBC`

2. **Input Simulation**:
   - Uses the `SendInput` Win32 API to simulate mouse and keyboard actions.
   - Left mouse button is held for blue or orange colors.
   - The "X" key is held for green or purple colors.

3. **Toggle Key**:
   - The `F8` key is used to toggle the bot on or off.
   - When toggled off, all simulated inputs are released.

4. **Console Output**:
   - Displays the bot's status (ON/OFF), the detected color, and the state of the simulated inputs (Z and X).
   - Includes a spinner for visual feedback.

## Requirements

- Windows operating system.
- .NET runtime.

## Usage

1. Clone the repository or download the source code.
2. Build the project using your preferred .NET development environment.  
    `dotnet build`
3. Run the compiled executable.  
    `dotnet run`
4. Move your mouse over the target area in the game.
5. Press `F8` to toggle the bot on or off.

## Notes

- The bot uses the current mouse cursor position to detect colors. You can optionally modify the code to use a fixed pixel coordinate.
- The bot continuously runs in a loop with a 1ms delay for real-time responsiveness.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

