**Description**

Othello game consists of Singleplayer and Multi-player modes, different user-oriented services.

The opponent of the user in singleplayer mode is *Handsome Jack*, the minimax based **very selfish** AI. Jack thinks he is the best player ever, and so it will be not easy to win the game.
There are 4 difficulties (behaviours) of Jack:
*  **- Easy**:       Jack chooses the cell to occupy randomly.
*  **- Medium**:     Jack tries to occupy as more cells as possible, thinking in the depth of 3.
*  **- Difficult**:  Handsome Jack tries to occupy as more cells as possible, knowing, that the best positions on the gameboard are corners. Depth of thinking is 5.
*  **- Impossible**: Jack behaves as in the *Difficult* mode, but with increased depth (7). Moreover, he cheats.
 

-In the Singleplayer mode in the web project, **Jack is so cool** that **Jack is even able to play with himself!** User only have to click his image continuously.

When the Singleplayer mode is on, it is possible to turn on/off the tips (usable cells) and *"hilarious  Jack's comments"* and change the colors of players in the web project.
The comments and rating, unfortunately, are not implemented in the web project due to the lack of time. Hovewer, they are already implemented in the console application.

There is a plenty of things to implement into this game in the future:
*  *Mixed* - behaviour mode (chooses the algorithm of thinking every time randomly)
*  *Statistics* - it would be great to add the statistics service into the game to trace all data about every game, such as average turn time, average numer of turns per game and so on.
*  *Graphics* - no doubts, that the graphical interface could be better
*  *Save/Load Game* - a service for storing and loaing unfinished games
*  *Alpha-Beta prunning* - an improvement of the used Jack's algorithm, where unnecessary branches are ommited in the process of *"thinking"*. It may help to decrease the time of calculating the position of the next move.


***ALL OF THESE WOULD HAVE BEEN IMPLEMENTED IF HAD MORE TIME***

Korotetskyi, 2020(c)