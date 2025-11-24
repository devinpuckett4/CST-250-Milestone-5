Devin Puckett

CST-250 Programming in C#2

Grand Canyon University

11/23/2025

Milestone 5

[https://github.com/devinpuckett4/CST-250-Milestone-5/blob/main/Milestone5.md]

https://www.loom.com/share/a64f8a9530cc494499310a094d6a2368











 

FLOW CHART

  
Figure 1: Flow chart of Milestone 5

This flowchart shows the full path of my Minesweeper game from start to finish. The game begins on FormStart, where the player chooses the board size and difficulty, then clicks Start Game to move into FormGame with the grid, timer, and optional peek feature. From there, the player clicks or flags cells, which triggers the RevealCell logic, flood fill for empty spaces, and a constant check to see if the game is won or lost. If the player hits a bomb, the flow goes to the Lose step, where all bombs are shown and the game ends. If the player successfully clears the board, it follows the Win path to display the score, prompt for their name, create a GameStat record, and update the High Scores screen with options to save, load, or sort scores before ending the game.



UML Class Diagram
  
Figure 2: UML Class Diagram

This diagram lays out how my Minesweeper game is organized using separate models, forms, and services. BoardModel and CellModel handle the core game data, including the grid, timer, difficulty, neighbors, and whether a cell is flagged, revealed, or a reward. GameState and GameStat track whether the player is still playing, won, or lost, and store the final results like score, time, and name. FormStart, FormGame, Form3, and Form4 control the user experience: starting the game, playing on the grid, entering the player’s name, and viewing or managing high scores. The IBoardOperations interface and BoardService class sit in the middle to perform actions like setting bombs, revealing cells, counting neighbors, toggling flags, and calculating the final score, keeping the game logic separate from the UI and easier to manage.


Low Fidelity

  
 
Figure 3: Screenshot of Build Success

This screenshot shows my full Minesweeper solution building successfully from the command line. After navigating into the Minesweeper folder, I run dotnet build and all projects compile with no errors. The Models and BLL projects succeed first, confirming that my core data structures and game logic are solid. The ConsoleApp, Tests, and WinForms projects all build cleanly as well, which means both interfaces and my automated tests are wired up correctly. Seeing “Build succeeded” at the bottom for every layer confirms the application is stable and ready to run or demonstrate.











High Fidelity
 

Figure 4: High Fidelity

 
This screenshot shows the results of running my automated tests for the Minesweeper solution using dotnet test. The Models, BLL, and Tests projects all build successfully first, confirming that the structure and references are set up correctly. xUnit then discovers and runs the Minesweeper.Tests project without any issues. The test summary reports four tests total, all succeeded with zero failures or skips, which gives me confidence that my core game logic is behaving as expected. Seeing “Build succeeded” at the end confirms that both compilation and testing passed cleanly, so the project is ready to demo.







 
 


Figure 5: Screenshot of Setup Form

 
This screenshot shows the Minesweeper setup screen configured for a large 24 x 24 board. The top slider is pushed all the way to the right, clearly displaying the selected board size beside it so the player knows they’re choosing a more advanced grid. The difficulty slider is set to 1, letting me demonstrate that players can combine a big board with an easier bomb density if they want more time to think. The layout keeps everything simple with just two sliders and a clear Start Game button so there’s no confusion about how to begin. It highlights that my game scales from small beginner boards up to much larger challenges using the same clean, user-friendly setup form.









Figure 6: Screenshot of 24x24 Board
 

This screenshot shows my Minesweeper game running on a large 24x24 board for a more advanced challenge. Every tile starts as a gray button with a question mark so it’s clear that everything is hidden and unexplored. The message at the top reminds the player of the simple controls: left-click to reveal and right-click to flag potential bombs. At the bottom, the Peeks display shows that one safe hint is available, along with Use and Close buttons so the player can decide when to spend it. The layout keeps everything clean and consistent, even at this bigger size, so the focus stays on strategy instead of clutter.





Figure 7: Screenshot of Visit, Fill and Flag

 

This screenshot shows my Minesweeper game running on a 24x24 board in the middle of an active round. A large portion of the board has been revealed, showing numbers and clear spaces that outline safe paths, while the remaining gray question mark tiles still hide possible bombs. A few yellow flags mark suspected bomb locations, helping me track my logic as I work through the grid. The instructions at the top keep controls clear, and at the bottom the Peeks display shows I still have one safe hint available with a Use button ready if I decide to rely on it. Even at this larger size, the layout stays readable and supports strategic play instead of guesswork.





 Figure 8: Screenshot of Loss

 
This screenshot shows the lose screen on my larger 24x24 Minesweeper board. The “Boom! You lost” message at the top, along with the final time of 411 seconds, clearly lets me know the round is over and how long I lasted. All bombs are revealed in red so I can see the full layout and where my mistakes were, while my incorrect guesses are easy to spot. The gray path of opened cells shows how far I had logically worked through the board before hitting a mine. At the bottom, the Peeks display and disabled Use button confirm that no more actions can be taken, and the Close button allows me to exit and start a new attempt. 








Figure 6: Screenshot of Error Handling

 
This screenshot shows my game catching a bad command. I typed 111 on purpose, and the app didn’t crash, it told me to enter three parts: row, col, action, with an example. That message comes from the checks I added in the main loop for empty input, token count, and parsing numbers. It reprompts right away, so I can try again without losing my place. This is quick proof that my input validation is working the way it should.














---
Use Case Scenario
---

 
This diagram shows the end-to-end user flow for my Minesweeper app, including the win path into high scores. From the User lane, I start the game, view the board, and make moves; those actions feed into Determine_Gamestate, which decides the result. The results screen shows win or loss, and on a win I’m prompted to enter my name. That name then flows into the View High Scores screen so the round is recorded and visible. The System lane covers engine actions like visit/flag/peek, and the Administrator lane handles setup and quality, setting board size, difficulty percentage, and running xUnit tests to keep everything reliable.

ADD ON
Programming Conventions
1.	Keep files grouped by what they do. Models hold the data, BLL handles the game rules, and the WinForms app shows the board and reads input.
2.	Name things clearly and add small comments so future me knows what a method is for.
3.	The BLL is the brain. The models are storage. The UI only displays and reads from the player.
4.	Keep methods focused on one job. RevealCell now handles regular visits and calls flood fill when needed. Other methods include SetupBombs, CountBombsNearby, ToggleFlag, UseRewardPeek, and DetermineGameState.
5.	Check for out-of-bounds or bad input first, then always guide the player with a friendly message.
Computer Specs
 • Windows 10 or Windows 11 
• Visual Studio 2022 • .NET SDK installed
• 8 GB RAM or more • Git and GitHub account
Work Log Milestone 5 Wednesday • 4:00–5:15 PM - Added GameStat model and started Form3 setup Total: 1h 15m
Wednesday - Discussion Day • Posted about high scores and JSON saving Total: 40m
Friday - Discussion Day • Posted about data persistence challenges with JSON Total: 40m
Friday • 6:15–7:30 PM - Added GameStat model and Form3 for name input • 7:40–8:30 PM - Created Form4 with grid, menu for sort/save/load Total: 1h 45m
Saturday • 10:00–11:15 AM - Updated UML and flowchart to include stats and new forms • 11:30–12:40 PM - Linked win event to Form3/Form4, added test for stat creation Total: 2h 25m
Saturday - Discussion Day • 1:00–1:35 PM - Discussion replies about data persistence in apps Total: 35m
Sunday • 5:20–6:45 PM - Fixed JSON path for saving, added debug messages • 7:00–7:40 PM - Tested high scores populate on win, retook screenshots Total: 2h 05m
Grand Total: 9h 20m
What I added 
• GameStat model for name/score/time/date 
• Form3 for name input on win 
• Form4 for high scores grid, sort/save/load with JSON 
• Updated BLL with CreateGameStat, linked to win in GUI
Research
1.	YouTube
2.	Geometry Dash
3.	Platformer
4.	Fun but hard; frustrating when you die a lot.
5.	Colorful and vibrant, attractive.
6.	Use level progression for adding harder Minesweeper modes.
OOP principles I used
 • Abstraction - The interface describes what the board service can do 
• Encapsulation - Stats data stays in GameStat, loaded/saved privately in Form4 
• Polymorphism - Sort methods use switch for different properties 
• Inheritance - New forms inherit from Form for UI consistency
Tests 
• Four tests passed: stat creation, JSON save/load, sort by score, win trigger
Bug Report 
• High scores not populating, fixed file path to Application.StartupPath 
• Form3 cutoff, increased height 
• Win not triggering forms, added debug to confirm state
Follow-Up Questions
1.	What was tough? Getting JSON to save/load right without errors.
2.	What did I learn? Persistence with JSON makes data last between runs, and sorting with LINQ is quick.
3.	How would I improve it? Add delete button for old scores or filter by date.
4.	How does this help at work? Saving user data like scores makes apps more engaging, and testing persistence avoids lost info bugs.

