// Author: Eric Pu
// File Name: Game1.cs
// Project Name: Elevens
// Creation Date: Feb 5, 2020
// Modified Date: Feb 16, 2020
// Description: A McDonald's themed version of the classic card game Elevens.

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Animation2D;

namespace Elevens
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        static Random rng = new Random();

        // Screen dimensions
        Vector2 screenDimensions;

        // Mouse states
        static MouseState mousePast = new MouseState();
        static MouseState mouseCurrent = Mouse.GetState();

        // Board state
        static List<string> deck;
        static string[,] board;
        static int[] stackSizes;
        static bool playerWon;
        static bool playerLost;

        // Highlighted card and selected cards are nullable since they might not exist at any point
        static int? highlightedCard;
        static int? selectedCard;

        // 2D array containing all possible pairs with sum of eleven
        static int[,] elevenPairs;

        // Dictionary containing corresponding int values for each suit
        static Dictionary<string, int> suits;

        // Dictionary containing rectangles of each card
        static Dictionary<int, Rectangle> cardRec;

        // Dimensions of the cards
        static int cardWidth;
        static int cardHeight;

        // Y coordinates of each row
        static int row1Y;
        static int row2Y;

        // Space between each card and starting coordinate
        static int spaceBetweenCards;
        static int rowStartX;

        // Thickness of card outlines
        static int outlineThickness;

        // Thickness of text drop shadow
        static int textShadowThickness;

        // Gamestate and gamestate constants
        static int gamestate;
        static int currentHelpScreen;
        const int INTRO = 0;
        const int MENU = 1;
        const int GAMEPLAY = 2;
        const int WIN = 3;
        const int LOSS = 4;
        const int HELP = 5;

        // Hidden mode settings
        static int hiddenDuration;
        static int hiddenTimer;
        static int hiddenCardsAmount;
        static List<int> cardsHidden;
        static bool hiddenModeActive;

        // Misc constants
        const int FRAME_RATE = 60;
        const int CARDS_PER_SUIT = 13;
        const int FIRST_FACE_CARD = 11;

        // Fade variables
        static int fadeDuration;
        static bool fadeActive;
        static int fadeTimer;
        static bool gamestateSwitched;
        static int fadeDestination;
        static float fadeOpacity;

        // Background image fade variables
        static int bgDuration;
        static int bgTimer;
        static int bgFadeDuration;
        static int bgFadeTimer;
        static bool bgSwitched;
        static float bgOpacity;
        static bool bgFadeActive;
        static int currentBg;
        static float bgBaseOpacity;

        // Intro variables
        static int introTimer;
        static int introDuration;
        static bool introOver;

        // Button and background rectangles
        Rectangle bgRec;
        Rectangle startBtnRec;
        Rectangle helpBtnRec;
        Rectangle menuBtnRec;
        Rectangle backBtnRec;
        Rectangle nextBtnRec;
        Rectangle hiddenBtnRec;

        // Sprites 
        Texture2D cardsImg;
        Texture2D blankRecImg;
        Texture2D cardBackImg;
        Texture2D[] bgImg;
        Texture2D[] helpBgImg;
        Texture2D lossImg;
        Texture2D winImg;
        Texture2D menuImg;
        Texture2D introImg;
        Texture2D cursorImg;
        Texture2D blankCardImg;

        // Button sprites
        Texture2D helpBtnImg;
        Texture2D startBtnImg;
        Texture2D menuBtnImg;
        Texture2D hiddenOffBtnImg;
        Texture2D hiddenOnBtnImg;
        Texture2D backBtnImg;
        Texture2D nextBtnImg;

        // Music and sound effects
        Song bgMusic;
        SoundEffect[] correctSnd;
        SoundEffect errorSnd;
        SoundEffect introSnd;
        SoundEffect winSnd;
        SoundEffect lossSnd;
        SoundEffect clickSnd;

        static bool endSndPlayed;

        // Animations
        Texture2D[] aniImg;
        static Animation[] animations;
        int[] animationsFrameCounts;
        Rectangle animationRec;
        static int currentAnimation;

        // Fonts
        SpriteFont bigFont; 

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // Sets screen dimensions
            screenDimensions = new Vector2(900, 600);
            graphics.PreferredBackBufferWidth = Convert.ToInt32(screenDimensions.X);
            graphics.PreferredBackBufferHeight = Convert.ToInt32(screenDimensions.Y);
            graphics.ApplyChanges();

            // Initializes mouse states
            mouseCurrent = Mouse.GetState();
            mousePast = new MouseState();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Animation sprite sheets
            aniImg = new Texture2D[]
            {
                Content.Load<Texture2D>("Images/Animations/runfast"),
                Content.Load<Texture2D>("Images/Animations/throwburger"),
                Content.Load<Texture2D>("Images/Animations/throwmanyburger")
            };

            // Loads images
            cardsImg = Content.Load<Texture2D>("Images/sprites/cardfaces");
            blankRecImg = Content.Load<Texture2D>("Images/sprites/blankrec");
            cardBackImg = Content.Load<Texture2D>("Images/sprites/cardback");

            // Background images during gameplay
            bgImg = new Texture2D[]
            {
                Content.Load<Texture2D>("Images/backgrounds/bg1"),
                Content.Load<Texture2D>("Images/backgrounds/bg2"),
                Content.Load<Texture2D>("Images/backgrounds/bg3"),
                Content.Load<Texture2D>("Images/backgrounds/bg4"),
                Content.Load<Texture2D>("Images/backgrounds/bg5")
            };

            // Help screen images
            helpBgImg = new Texture2D[]
            {
                Content.Load<Texture2D>("Images/backgrounds/help1"),
                Content.Load<Texture2D>("Images/backgrounds/help2"),
                Content.Load<Texture2D>("Images/backgrounds/help3"),
                Content.Load<Texture2D>("Images/backgrounds/help4")
            };

            // Other background images
            lossImg = Content.Load<Texture2D>("Images/backgrounds/lose");
            winImg = Content.Load<Texture2D>("Images/backgrounds/win");
            menuImg = Content.Load<Texture2D>("Images/backgrounds/menu");
            introImg = Content.Load<Texture2D>("Images/backgrounds/intro");

            // Button sprites
            helpBtnImg = Content.Load<Texture2D>("Images/sprites/helpbutton");
            startBtnImg = Content.Load<Texture2D>("Images/sprites/startbutton");
            menuBtnImg = Content.Load<Texture2D>("Images/sprites/menubutton");
            hiddenOnBtnImg = Content.Load<Texture2D>("Images/sprites/hiddenonbutton");
            hiddenOffBtnImg = Content.Load<Texture2D>("Images/sprites/hiddenoffbutton");
            nextBtnImg = Content.Load<Texture2D>("Images/sprites/nextbutton");
            backBtnImg = Content.Load<Texture2D>("Images/sprites/backbutton");

            // Cursor and blank card images
            cursorImg = Content.Load<Texture2D>("Images/sprites/cursor");
            blankCardImg = Content.Load<Texture2D>("Images/sprites/blankcard");

            // Loads font
            bigFont = Content.Load<SpriteFont>("Fonts/bigtext");

            // Loads sound effects and song
            bgMusic = Content.Load<Song>("Audio/music");
            lossSnd = Content.Load<SoundEffect>("Audio/loss");
            winSnd = Content.Load<SoundEffect>("Audio/win");
            errorSnd = Content.Load<SoundEffect>("Audio/error");
            introSnd = Content.Load<SoundEffect>("Audio/intro");
            clickSnd = Content.Load<SoundEffect>("Audio/buttonclick");

            // Correct match soudn effects
            correctSnd = new SoundEffect[]
            {
                Content.Load<SoundEffect>("Audio/correct1")
                ,Content.Load<SoundEffect>("Audio/correct2")
                ,Content.Load<SoundEffect>("Audio/correct3")
            };

            // Number of frames in each animation
            animationsFrameCounts = new int[] { 61, 31, 48 };

            // Initializes all 3 animations
            animations = new Animation[]
            {
                new Animation(aniImg[0], 5, 13, animationsFrameCounts[0], 1, 1, Animation.ANIMATE_FOREVER, 6, new Vector2(animationRec.X, animationRec.Y), 1, true),
                new Animation(aniImg[1], 5, 7, animationsFrameCounts[1], 1, 1, Animation.ANIMATE_FOREVER, 4, new Vector2(animationRec.X, animationRec.Y), 1, true),
                new Animation(aniImg[2], 5, 10, animationsFrameCounts[2], 1, 1, Animation.ANIMATE_FOREVER, 4, new Vector2(animationRec.X, animationRec.Y), 1, true),
            };

            // Rectangle of all animations
            animationRec = new Rectangle(320, 420, 300, 150);

            // Sets destRec for each animation, and sets them to not animating
            for (int i = 0; i < animations.Length; ++i)
            {
                animations[i].destRec = animationRec;
                animations[i].isAnimating = false;
            }

            // Defines elevenPairs as 2D array of every possible pair with sum of 11
            elevenPairs = new int[,]
            {
                { 10, 1 },
                { 9, 2 },
                { 8, 3 },
                { 7, 4 },
                { 6, 5 }
            };

            // Sets dimensions of card image
            cardWidth = 130;
            cardHeight = 195;

            // Y coordinates of both rows
            row1Y = 15;
            row2Y = 230;

            // Starting X coordinate of each row
            rowStartX = 35;

            // Space between each card image
            spaceBetweenCards = 10;

            // Sits number of cards hidden at once to 6
            hiddenCardsAmount = 6;
            cardsHidden = new List<int>();

            // Hidden lasts 120 frames (2 seconds)
            hiddenDuration = FRAME_RATE * 2;
            hiddenTimer = hiddenDuration;
            hiddenModeActive = false;

            // Resets fade settings, fade lasts 60 frames (1 second)
            fadeActive = false;
            fadeDuration = FRAME_RATE;
            gamestateSwitched = true;
            fadeTimer = fadeDuration;
            fadeOpacity = 0f;

            // Resets intro settings
            // Intro lasts 60 frames (1 second)
            introDuration = FRAME_RATE;
            introTimer = introDuration;
            introOver = false;

            // Resets intro background settings
            // Background lasts 10 seconds each
            bgDuration = 10 * FRAME_RATE;
            bgTimer = bgDuration;

            // Fade lasts 30 frames (.5 seconds)
            bgFadeDuration = 30;
            bgFadeTimer = bgFadeDuration;
            bgSwitched = false;
            bgFadeActive = false;
            currentBg = 0;

            // Opacity of background is .6f
            bgOpacity = .6f;
            bgBaseOpacity = .6f;

            // Creates rectangles for buttons
            startBtnRec = new Rectangle(350, 500, 200, 80);
            helpBtnRec = new Rectangle(50, 500, 200, 80);
            menuBtnRec = new Rectangle(350, 400, 200, 80);
            hiddenBtnRec = new Rectangle(650, 500, 200, 80);
            backBtnRec = new Rectangle(50, 500, 200, 80);
            nextBtnRec = new Rectangle(650, 500, 200, 80);

            // Sets rectangle of background image to entire screen
            bgRec = new Rectangle(0, 0, Convert.ToInt32(screenDimensions.X), Convert.ToInt32(screenDimensions.Y));

            // Sets corresponding int value for each suit, as according to the layout of the card images
            suits = new Dictionary<string, int>
            {
                {"D", 0 },
                {"H", 1 },
                {"C", 2 },
                {"S", 3 }
            };

            // Gamestate starts at intro screen
            gamestate = INTRO;

            // Sets thickness of image outlines
            outlineThickness = 10;

            // Sets thickness of the text drop shadows
            textShadowThickness = 4;

            // Resets game settings
            ResetGame();

            // Initializes dictionary containing rectangles for each card
            cardRec = new Dictionary<int, Rectangle>();

            // Iterates through each card, creating rectangle for corresponding row
            for (int i = 0; i < board.Length; ++i)
            {
                // If card is in the first row, the card has the first row Y coordinate
                // Otherwise, the card has the coordinate of the second row
                if (i < board.GetLength(1))
                {
                    // X is equal to the starting X coordinate + i * (card width + space between each card)
                    cardRec.Add(i, new Rectangle(rowStartX + i * (cardWidth + spaceBetweenCards), row1Y, cardWidth, cardHeight));
                }
                else
                {
                    // For the second row, the length of the second dimension must be subtracted from i
                    cardRec.Add(i, new Rectangle(rowStartX + (i - board.GetLength(1)) * (cardWidth + spaceBetweenCards), row2Y, cardWidth, cardHeight));
                }
            }

            // Sets audio settings and plays music
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.2f;
            MediaPlayer.Play(bgMusic);

            // Plays intro sound effect
            introSnd.Play();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            // Updates mouse states
            mousePast = mouseCurrent;
            mouseCurrent = Mouse.GetState();

            // Updates the fading screen
            UpdateFade();

            // Logic for each game state
            switch (gamestate)
            {
                case INTRO:
                    {
                        // Updates the timer of the game's intro screen
                        --introTimer;

                        // If the intro timer is less than 0, and the intro has not ended yet, the fade to the menu begins and introOver is true
                        if (introTimer < 0 && !introOver)
                        {
                            StartFade(MENU);
                            introOver = true;
                        }
                        break;
                    }
                case MENU:
                    {
                        // Checks the start button, and fades to gameplay if clicked
                        if (CheckButton(startBtnRec) && !fadeActive)
                        {
                            StartFade(GAMEPLAY);
                            clickSnd.Play();
                            ResetGame();
                        }

                        // Checks the help button, and fades to help screen if clicked
                        if (CheckButton(helpBtnRec) && !fadeActive)
                        {
                            StartFade(HELP);
                            clickSnd.Play();

                            // Resets the help screen to the first image
                            currentHelpScreen = 0;
                        }

                        // If the player clicks the hidden mode button, hidden mode is turned on/off
                        if (CheckButton(hiddenBtnRec))
                        {
                            if (hiddenModeActive) hiddenModeActive = false;
                            else hiddenModeActive = true;

                            clickSnd.Play();
                        }

                        break;
                    }
                case GAMEPLAY:
                    {
                        // Checks if an animation is currently playing
                        if (IsAnimationPlaying(animations))
                        {
                            // Checks if the animation has ended by seeing if the current frame is less than the total number of frames
                            // If it is less, the animation is updated as it has not ended yet
                            // Otherwise, the animation ends and the currentAnimation is incremented
                            if (animations[currentAnimation].curFrame < animationsFrameCounts[currentAnimation] - 1)
                            {
                                animations[currentAnimation].Update(gameTime);
                            }
                            else
                            {
                                // Indicates that animation has ended
                                animations[currentAnimation].isAnimating = false;

                                // If currentAnimation surpasses number of total animations, it is reset back to 0
                                // Otherwise, it is incremented
                                if (currentAnimation == animations.Length - 1)
                                {
                                    currentAnimation = 0;
                                }
                                else ++currentAnimation;
                            }
                        }

                        // Highlighted card is set to null with each update
                        highlightedCard = null;

                        // If the player hasn't lost already, and has a losing board state, game transitions to loss screen
                        // Else, if the player hasn't won already, the logic for the core gameplay is executed
                        if (!playerLost && CheckLossCondition())
                        {
                            StartFade(LOSS);
                            playerLost = true;
                        }
                        else if (!playerWon)
                        {
                            // Checks if hidden mode is currently active
                            if (hiddenModeActive)
                            {
                                // Updates the hidden timer
                                --hiddenTimer;

                                // If the hidden timer reaches 0, the currently hidden cards change
                                if (--hiddenTimer <= 0)
                                {
                                    // Resets hidden timer
                                    hiddenTimer = hiddenDuration;

                                    // Creates new list of hidden cards
                                    cardsHidden = new List<int>();

                                    // While the hidden cards list is not full, cards are added to the list
                                    while (cardsHidden.Count() < hiddenCardsAmount)
                                    {
                                        // Generates a random card within the board
                                        int card = rng.Next(board.Length);

                                        // If the list of hidden cards does not already contain this card, it is added
                                        if (!cardsHidden.Contains(card)) cardsHidden.Add(card);
                                    }
                                }
                            }

                            // Loops through every card's rectangle, checking if the mouse is within it
                            // If it is, the highlighted card is set to that card, and the loop ends
                            for (int i = 0; i < cardRec.Count(); ++i)
                            {
                                if (MouseInRec(cardRec[i]))
                                {
                                    highlightedCard = i;
                                    break;
                                }
                            }

                            // If the highlighted card is not null (player is hovering over any card) and player clicks, card matching logic is executed
                            if (highlightedCard != null && CheckLeftClick())
                            {
                                // If there is no selected card, card selection logic is executed
                                // Otherwise, elevens matching logic is executed
                                if (selectedCard == null)
                                {
                                    // If the highlighted card is a swappable face card, it is swapped
                                    // If the card is not a face card, the highlighted card becomes the selected card
                                    if (CheckFace(highlightedCard.Value)) SwapFace(highlightedCard.Value);
                                    else if (GetCardValue(highlightedCard.Value) < FIRST_FACE_CARD) selectedCard = highlightedCard;

                                    clickSnd.Play();
                                }
                                else
                                {
                                    // Checks if the two cards are a valid pair of eleven
                                    if (CheckEleven(selectedCard.Value, highlightedCard.Value))
                                    {
                                        // If the deck has two cards left, the player wins 
                                        // (since the two cards would be removed, resulting in all face cards on the board)
                                        // Otherwise, the top two cards from the deck are removed and placed on the corresponding piles
                                        if (deck.Count() == 2)
                                        {
                                            StartFade(WIN);
                                            playerWon = true;
                                        }
                                        else
                                        {
                                            // If the selected card is in the first row, the corresponding card in the second row is set to the first card in the deck
                                            // Otherwise, card in first row is set to first card in deck
                                            if (selectedCard.Value >= board.GetLength(1))
                                            {
                                                board[1, selectedCard.Value - board.GetLength(1)] = deck[0];
                                            }
                                            else board[0, selectedCard.Value] = deck[0];

                                            // Removes the first card in the deck
                                            deck.RemoveAt(0);

                                            // Same logic as above, but for the highlighted card
                                            if (highlightedCard.Value >= board.GetLength(1))
                                            {
                                                board[1, highlightedCard.Value - board.GetLength(1)] = deck[0];
                                            }
                                            else board[0, highlightedCard.Value] = deck[0];

                                            deck.RemoveAt(0);

                                            // Increases stack sizes of both highlighted and selected pile
                                            ++stackSizes[selectedCard.Value];
                                            ++stackSizes[highlightedCard.Value];

                                            // Plays a random correct sound effect
                                            correctSnd[rng.Next(correctSnd.Length)].Play();

                                            // Checks if an animation is not currently playing
                                            if (!IsAnimationPlaying(animations))
                                            {
                                                // Sets the current animation's isAnimating to true
                                                animations[currentAnimation].isAnimating = true;

                                                // Resets the animation by setting current frame to 0
                                                animations[currentAnimation].curFrame = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // If the pair is invalid, an error sound is played
                                        errorSnd.Play();
                                    }
                                    // Resets the selected card
                                    selectedCard = null;
                                }
                            }
                        }
                        break;
                    }
                case WIN:
                    {
                        // if the end sound has not played yet, it is played and endSndPlayed is set to true
                        if (!endSndPlayed)
                        {
                            winSnd.Play();
                            endSndPlayed = true;
                        }

                        // If the menu button is clicked, the game returns to the menu
                        if (CheckButton(menuBtnRec) && !fadeActive)
                        {
                            StartFade(MENU);
                            clickSnd.Play();
                        }

                        break;
                    }
                case LOSS:
                    {
                        // if the end sound has not played yet, it is played and endSndPlayed is set to true
                        if (!endSndPlayed)
                        {
                            lossSnd.Play();
                            endSndPlayed = true;
                        }

                        // If the menu button is clicked, the game returns to the menu
                        if (CheckButton(menuBtnRec) && !fadeActive)
                        {
                            StartFade(MENU);
                            clickSnd.Play();
                        }
                        break;
                    }
                case HELP:
                    {
                        // Checks if the back button is being clicked
                        if (CheckButton(backBtnRec))
                        {
                            // If the first help screen is being drawn, back button returns to menu
                            // Otherwise, current help screen is decremented
                            if (currentHelpScreen == 0) StartFade(MENU);
                            else --currentHelpScreen;
                            
                            // Plays click sound
                            clickSnd.Play();
                        }

                        // If the next button is pressed, and the current help screen is not the last one, currentHelpScreen is incremented and plays click sound
                        if (CheckButton(nextBtnRec) && currentHelpScreen < helpBgImg.Length - 1)
                        {
                            ++currentHelpScreen;
                            clickSnd.Play();
                        }

                        break;
                    }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            switch (gamestate)
            {
                case INTRO:
                    {
                        // Draws the intro image
                        spriteBatch.Draw(introImg, bgRec, Color.White);
                        break;
                    }
                case MENU:
                    {
                        // Draws the menu background image
                        spriteBatch.Draw(menuImg, bgRec, Color.White);

                        // Draws outlines for each of the menu buttons
                        if (MouseInRec(startBtnRec)) spriteBatch.Draw(blankCardImg, GetOutlineRec(startBtnRec), Color.Yellow);
                        if (MouseInRec(helpBtnRec)) spriteBatch.Draw(blankCardImg, GetOutlineRec(helpBtnRec), Color.Yellow);
                        if (MouseInRec(hiddenBtnRec)) spriteBatch.Draw(blankCardImg, GetOutlineRec(hiddenBtnRec), Color.Yellow);

                        // Draws the buttons
                        spriteBatch.Draw(startBtnImg, startBtnRec, Color.White);
                        spriteBatch.Draw(helpBtnImg, helpBtnRec, Color.White);

                        // If hidden mode is active, the on button is drawn
                        // Otherwise, the off button is drawn
                        if (hiddenModeActive) spriteBatch.Draw(hiddenOnBtnImg, hiddenBtnRec, Color.White);
                        else spriteBatch.Draw(hiddenOffBtnImg, hiddenBtnRec, Color.White);

                        break;
                    }
                case GAMEPLAY:
                    {
                        // Updates the background image
                        UpdateBackground();

                        // Draws the background image
                        spriteBatch.Draw(bgImg[currentBg % bgImg.Count()], bgRec, Color.White * bgOpacity);

                        // If an animation is playing, the animation is drawn
                        if (IsAnimationPlaying(animations)) animations[currentAnimation].Draw(spriteBatch, Color.White, SpriteEffects.None);

                        // If there is a selected card, a green outline is drawn around that card
                        if (selectedCard != null)
                        {
                            spriteBatch.Draw(blankCardImg, GetOutlineRec(cardRec[selectedCard.Value]), Color.Green);
                        }

                        // If there is a highlighted card, and that highlighted card is not the same as the selected card, a yellow outline is drawn
                        if (highlightedCard != null && selectedCard != highlightedCard)
                        {
                            spriteBatch.Draw(blankCardImg, GetOutlineRec(cardRec[highlightedCard.Value]), Color.Yellow);
                        }

                        // Loops through each board element
                        for (int i = 0; i < board.Length; ++i)
                        {
                            // Checks if the card is a face card, and if the stack contains more than one card
                            if (GetCardValue(i) >= FIRST_FACE_CARD && stackSizes[i] > 1)
                            {
                                // Draws a red outline around the face card
                                spriteBatch.Draw(blankCardImg, GetOutlineRec(cardRec[i]), Color.Red);
                            }

                            // If the hidden mode is currently on, and the current card is hidden, then a blank card is drawn
                            // Otherwise, the corresponding card is drawn
                            if (hiddenModeActive && cardsHidden.Contains(i))
                            {
                                // Draws a blank card image for that card
                                spriteBatch.Draw(blankCardImg,
                                    cardRec[i],
                                    Color.White);
                            }
                            else
                            {
                                // Draws the card image
                                spriteBatch.Draw(cardsImg,
                                    cardRec[i],
                                    GetCardImage(i), Color.White);
                            }
                        }

                        // Draws cards remaining, with a drop shadow on the text
                        spriteBatch.DrawString(bigFont, "DECK: " + Convert.ToString(deck.Count()), new Vector2(10 + textShadowThickness, 540 + textShadowThickness), Color.Black);
                        spriteBatch.DrawString(bigFont, "DECK: " + Convert.ToString(deck.Count()), new Vector2(10, 540), Color.White);

                        break;
                    }
                case WIN:
                    {
                        // Draws the win background image
                        spriteBatch.Draw(winImg, bgRec, Color.White);

                        // Draws outline for menu button
                        if (MouseInRec(menuBtnRec)) spriteBatch.Draw(blankCardImg, GetOutlineRec(menuBtnRec), Color.Yellow);

                        // Draws the menu button
                        spriteBatch.Draw(menuBtnImg, menuBtnRec, Color.White);

                        break;
                    }
                case LOSS:
                    {
                        // Draws the loss background image
                        spriteBatch.Draw(lossImg, bgRec, Color.White);

                        // Draws outline for menu button
                        if (MouseInRec(menuBtnRec)) spriteBatch.Draw(blankCardImg, GetOutlineRec(menuBtnRec), Color.Yellow);

                        // Draws the menu button
                        spriteBatch.Draw(menuBtnImg, menuBtnRec, Color.White);

                        break;
                    }
                case HELP:
                    {
                        // Draws the current help screen image
                        spriteBatch.Draw(helpBgImg[currentHelpScreen], bgRec, Color.White);

                        // Draws outline for back button
                        if (MouseInRec(backBtnRec))
                        {
                            spriteBatch.Draw(blankCardImg, GetOutlineRec(backBtnRec), Color.Yellow);
                        }

                        // Draws outline for next button if not on the last help screen
                        if (MouseInRec(nextBtnRec) && currentHelpScreen < helpBgImg.Length - 1)
                        {
                            spriteBatch.Draw(blankCardImg, GetOutlineRec(nextBtnRec), Color.Yellow);
                        }

                        // If the current help screen is not the last one, the next button is drawn
                        if (currentHelpScreen < helpBgImg.Length - 1) spriteBatch.Draw(nextBtnImg, nextBtnRec, Color.White);
                        
                        // If the current help screen isn't the last one, the back button is drawn
                        // Otherwise, the menu button is drawn
                        if (currentHelpScreen != 0) spriteBatch.Draw(backBtnImg, backBtnRec, Color.White);
                        else spriteBatch.Draw(menuBtnImg, backBtnRec, Color.White);

                        break;
                    }
            }

            // If the fade is active, draws the black screen fade image
            if (fadeActive) spriteBatch.Draw(blankRecImg, bgRec, Color.Black * fadeOpacity);

            // Draws the cursor image
            spriteBatch.Draw(cursorImg, new Rectangle(mouseCurrent.X, mouseCurrent.Y, 40, 40), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Pre: List of strings being shuffled
        /// Post: Returns the shuffled list
        /// Description: Shuffles a list of strings
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<string> ShuffleList(List<string> list)
        {
            // Sets n to the number of elements in the list
            int n = list.Count;
            
            // Loops through this shuffling process n times
            for (int i = 0; i < n; ++i)
            {
                // Generates a random index within the list
                int randomIndex = rng.Next(n-1);

                // Sets firstElement as the nth element, secondElement as the random element
                string firstElement = list[i];
                string secondElement = list[randomIndex];

                // Swaps the two elements
                list[randomIndex] = firstElement;
                list[i] = secondElement;
            }

            // Returns the list
            return list;
        }

        /// <summary>
        /// Pre: int card as the card's position on the board
        /// Post: Returns a bool indicating if the card is a valid face or not
        /// Description: Checks if a card is a valid face card to be removed
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static bool CheckFace(int card)
        {
            // If there is only one card in the pile, and the card's value is that of a face card, returns true
            // Otherwise, returns false
            if (stackSizes[card] == 1 && GetCardValue(card) >= FIRST_FACE_CARD)
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Pre: int card as the card's position on the board
        /// Post: n/a
        /// Description: Replaces a face card on the board
        /// </summary>
        /// <param name="card"></param>
        public static void SwapFace(int card)
        {
            // Adds the face card to the bottom of the deck
            deck.Add(GetCard(card));

            // Checks if the card is in the first or second row
            if (card >= board.GetLength(1))
            {
                // Replaces the card with the top of the deck, then removes that card
                board[1, card - board.GetLength(1)] = deck[0];
                deck.RemoveAt(0);
            }
            else
            {
                // Replaces the card with the top of the deck, then removes that card
                board[0, card] = deck[0];
                deck.RemoveAt(0);
            }
        }

        /// <summary>
        /// Pre: int card1 as the first card, int card2 as the second card
        /// Post: Returns a bool indicating if the pair is an eleven or not
        /// Description: Checks if a pair of cards are a pair of eleven
        /// </summary>
        /// <param name="card1"></param>
        /// <param name="card2"></param>
        /// <returns></returns>
        public static bool CheckEleven(int card1, int card2)
        {
            // Gets value of both cards, and returns true if they add up to eleven
            // Otherwise, returns false
            if (GetCardValue(card1) + GetCardValue(card2) == 11) return true;
            else return false;
        }

        /// <summary>
        /// Pre: n/a
        /// Post: Returns a bool indicating if the player has lost or not
        /// Description: Checks if the player has lost or not
        /// </summary>
        public static bool CheckLossCondition()
        {
            // Creates a new list with the values of each card
            List<int> boardVals = new List<int>();

            // Loops through every card on the board
            for (int i = 0; i < board.Length; ++i)
            {
                // Creates and sets cardVal to the value of the current card
                int cardVal = GetCardValue(i);

                // If there is only one card in the pile, and card is a face card, returns false as player still has moves
                if (stackSizes[i] == 1 && cardVal >= FIRST_FACE_CARD) return false;

                // Adds value of the card to boardVals
                boardVals.Add(cardVal);
            }

            // Loops through each possible pair of elevens
            for (int i = 0; i < elevenPairs.GetLength(0); ++i)
            {
                // If board contains cards with both values of the pair, returns false as player still has moves
                if (boardVals.Contains(elevenPairs[i, 0]) && boardVals.Contains(elevenPairs[i, 1])) return false;
            }

            // If neither false clauses are met, returns true as player has no other moves available
            return true;
        }

        /// <summary>
        /// Pre: int card as the position of the card on the board
        /// Post: Returns the rectangle for the source image of the card
        /// Description: Provides the source rectangle for a corresponding card on the board
        /// </summary>
        public static Rectangle GetCardImage(int card)
        {
            return new Rectangle(

                // Multiplies the value of the card - 1 (because cards start at 1, not 0) and multiplies by card width
                (GetCardValue(card) - 1) * cardWidth, 
                
                // Gets integer value of card's suit and multiplies by height
                GetSuit(card) * cardHeight, 

                // Sets source rec's width and height to dimensions of card
                cardWidth, 
                cardHeight);
        }

        /// <summary>
        /// Pre: Rectangle rec as the rectangle being checked
        /// Post: Returns a bool indicating if the mouse is within the rectangle
        /// Description: Checks if the player's mouse is currently in a rectangle
        /// </summary>
        public static bool MouseInRec(Rectangle rec)
        {
            // If the mouse is currently within the rectangle, returns true
            // Otherwise, returns false
            if (rec.X + rec.Width >= mouseCurrent.X && rec.X <= mouseCurrent.X
                && rec.Y + rec.Height >=mouseCurrent.Y && rec.Y <= mouseCurrent.Y) return true;
            else return false;
        }

        /// <summary>
        /// Pre: n/a
        /// Post: Returns a bool indicating if player is left clicking or not
        /// Description: Checks if the player is left clicking or not
        /// </summary>
        public static bool CheckLeftClick()
        {
            // Checks if the mouse is currently pressing left click and was not in the past
            if (mouseCurrent.LeftButton == ButtonState.Pressed && mousePast.LeftButton != ButtonState.Pressed) return true;
            else return false;
        }

        /// <summary>
        /// Pre: Rectangle rec as the rectangle of the button being checked
        /// Post: Returns a bool indicating if the player is clicking the button or not
        /// Description: Checks if the player is currently clicking a button
        /// </summary>
        public static bool CheckButton(Rectangle rec)
        {
            // If the mouse is in the button's rectangle, and player is left clicking, returns true
            // Otherwise, returns false
            if (MouseInRec(rec) && CheckLeftClick()) return true;
            else return false;
        }

        /// <summary>
        /// Pre: int card as the card's position on the board
        /// Post: Returns the rectangle of the corresponding card's outline
        /// Description: Provides the rectangle for the outline of a highlighted/selected/invalid card on the board
        /// </summary>
        public static Rectangle GetOutlineRec(Rectangle rec)
        {
            // Returns rectangle of the corresponding card's outline
            return new Rectangle(

                // Sets X and Y values as card's X/Y - half outline's thickness
                rec.X - outlineThickness / 2, 
                rec.Y - outlineThickness / 2,

                // Sets width and height as card's width/height + outline's thickness
                rec.Width + outlineThickness, 
                rec.Height + outlineThickness);
        }

        /// <summary>
        /// Pre: int card as the card on the board being retrieved
        /// Post: Returns int as the value of that card
        /// Description: Retrieves the value of a card, given its position on the board
        /// </summary>
        public static int GetCardValue(int card)
        {
            // First gets the string of the corresponding card on the board
            // Slices the string into the first two characters, and convents it to an integer
            // REturns this integer value as the card value
            return Convert.ToInt32(GetCard(card).Substring(0, 2));
        }

        /// <summary>
        /// Pre: int card as the card on the board being retrieved
        /// Post: Returns int as the suit of that card
        /// Description: Retrieves the suit of a card, given its position on the board
        /// </summary>
        public static int GetSuit(int card)
        {
            // First gets the string of the corresponding card on the board
            // Slices the string of the card to the last character, which is the suit
            // Returns the integer value of the suit, given the key (the string indicating the suit)
            return suits[GetCard(card).Substring(2, 1)];
        }

        /// <summary>
        /// Pre: n/a
        /// Post: n/a
        /// Description: Resets the game's settings and board 
        /// </summary>
        public static void ResetGame()
        {
            // Resets the deck
            deck = new List<string>();

            // Adds a card of each suit, for each card value
            for (int i = 1; i <= CARDS_PER_SUIT; ++i)
            {
                // If the card value is less than 10 (single digit), a 0 is added (ensures all card strings are 3 characters)
                // Adds to the deck a string for each suit, with the corresponding value
                if (i < 10)
                {
                    deck.Add("0" + Convert.ToString(i) + "D");
                    deck.Add("0" + Convert.ToString(i) + "H");
                    deck.Add("0" + Convert.ToString(i) + "C");
                    deck.Add("0" + Convert.ToString(i) + "S");
                }
                else
                {
                    deck.Add(Convert.ToString(i) + "D");
                    deck.Add(Convert.ToString(i) + "H");
                    deck.Add(Convert.ToString(i) + "C");
                    deck.Add(Convert.ToString(i) + "S");
                }
            }

            // Shuffles the deck
            ShuffleList(deck);

            // Creates a new board
            board = new string[2, 6];

            // Places a card in each column for both rows, and removes each card from the deck
            for (int i = 0; i < board.GetLength(1); ++i)
            {
                board[0, i] = deck[0];
                deck.RemoveAt(0);

                board[1, i] = deck[0];
                deck.RemoveAt(0);
            }

            // Creates new stack sizes
            stackSizes = new int[12];

            // Sets size of all stacks to 1, as each pile has only 1 card at the start
            for (int i = 0; i < stackSizes.Length; ++i)
            {
                stackSizes[i] = 1;
            }

            // Sets player's win/loss status to false
            playerWon = false;
            playerLost = false;

            // Resets the selected card
            selectedCard = null;

            // Resets the end screen sound being played
            endSndPlayed = false;

            // Resets current animation
            currentAnimation = 0;

            // Resets animations 
            for (int i = 0; i < animations.Length; ++i)
            {
                animations[i].curFrame = 0;
                animations[i].isAnimating = false;
            }
        }

        /// <summary>
        /// Pre: int destination as the game state the program will transition to
        /// Post: n/a
        /// Description: Starts a fade, which transitions into the given game state 
        /// </summary>
        public static void StartFade(int destination)
        {
            // Sets the destination game state to the parameter
            fadeDestination = destination;
            
            // Resets settings of the fade
            fadeTimer = fadeDuration;
            gamestateSwitched = false;
            fadeOpacity = 0f;
            fadeActive = true;
        }

        /// <summary>
        /// Pre: n/a
        /// Post: n/a
        /// Description: Updates the fading screen between game states 
        /// </summary>
        public static void UpdateFade()
        {
            // Checks if the fade is currently active
            if (fadeActive)
            {
                // Checks if the game state has transitioned yet
                if (!gamestateSwitched)
                {
                    // Updates the fade timer
                    --fadeTimer;

                    // Increases the opacity (as the screen is turning black) by base opacity / frame rate
                    fadeOpacity += (1f / FRAME_RATE);

                    // If the fade timer reaches 0, the game state changes
                    if (fadeTimer <= 0)
                    {
                        // Changes game state and indicates game state has changed
                        gamestate = fadeDestination;
                        gamestateSwitched = true;

                        // Resets the fade timer
                        fadeTimer = fadeDuration;
                    }
                }
                else
                {
                    // Updates the fade timer
                    --fadeTimer;

                    // Reduces the opacity (as the screen returns to normal)
                    fadeOpacity -= (1f / FRAME_RATE);

                    // If the fade timer reaches 0, the fade ends
                    if (fadeTimer <= 0) fadeActive = false;
                }
            }
        }

        /// <summary>
        /// Pre: int card as the corresponding card's position on the board
        /// Post: Returns the string of the corresponding card
        /// Description: Provides the string of a card, given the corresponding pile
        /// </summary>
        public static string GetCard(int card)
        {
            // If the card is within the first row, function returns the card in the first row
            // Otherwise, returns the card in the second row
            if (card < board.GetLength(1)) return board[0, card];
            else return board[1, card - board.GetLength(1)];
        }

        /// <summary>
        /// Pre: n/a
        /// Post: n/a
        /// Description: Updates the background image, and initiates/updates the fade if applicable
        /// </summary>
        public static void UpdateBackground()
        {
            // Checks if the fade is currently active or not
            // if it isn't, the timer for the current background image is updated
            // If it is active, the fade is updated
            if (!bgFadeActive)
            {
                // Updates the timer of the background image
                --bgTimer;

                // if the timer reaches 0, the fade begins and timer is reset
                if (bgTimer <= 0)
                {
                    bgFadeActive = true;
                    bgTimer = bgDuration;
                }
            }
            else
            {
                // If the background hasn't changed yet, the background image's opacity decreases (screen gets darker)
                // If it has, opacity increases (screen gets less dark)
                if (!bgSwitched)
                {
                    // Updates the timer of the fade
                    --bgFadeTimer;

                    // Decreases opacity by the base opacity / the duration of the fade (in frames)
                    bgOpacity -= (bgBaseOpacity / bgFadeDuration);

                    // If the timer reaches 0, the background image switches and the timer is reset
                    if (bgFadeTimer <= 0)
                    {
                        // Indicates image has switched, and resets timer
                        bgSwitched = true;
                        bgFadeTimer = bgFadeDuration;

                        // Increases the value of the current background image
                        ++currentBg;
                    }
                }
                else
                {
                    // Updates timer of the fade
                    --bgFadeTimer;

                    // increases opacity by base opacity / duration of fade
                    bgOpacity += (bgBaseOpacity / bgFadeDuration);

                    // If timer reaches 0, the fade ends and fade settings reset
                    if (bgFadeTimer <= 0)
                    {
                        // Resets fade settings
                        bgFadeActive = false;
                        bgSwitched = false;

                        // Resets timer
                        bgFadeTimer = bgFadeDuration;
                    }
                }
            }
        }

        /// <summary>
        /// Pre: Array of animations anis, containing animations being checked
        /// Post: Returns boolean indicating if animation is playing or not
        /// Description: Checks if an animation is currently playing within an array
        /// </summary>
        /// <param name="anis"></param>
        /// <returns></returns>
        public static bool IsAnimationPlaying(Animation[] anis)
        {
            // Iterates through each animation in the array
            // if the animation is animating, returns true
            // Otherwise, returns false at the end
            foreach (Animation ani in anis)
            {
                if (ani.isAnimating) return true;
            }
            return false;
        }
    }
}
