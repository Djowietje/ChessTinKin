using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessTinKin
{
    class ChessCode
    {
        ChessUI UI;
        
        int yOffset = 24;
       
        int chessWidth = 560;
        int chessHeight = 560;

        ChessPiece[] WhitePieces = new ChessPiece[16];
        ChessPiece[] BlackPieces = new ChessPiece[16];

        public void Run(ChessUI _UI)
        {
            UI = _UI;
            initDefaultLayout();
            

        }


        public void initDefaultLayout()
        {
            //Background
            Image bgImage = Image.FromFile("Pictures/ChessBoard.png");
            Image scaledBG = scaleImage(bgImage, chessWidth, chessHeight);
            


            Console.WriteLine("BG Image =" + scaledBG.Height + "," + scaledBG.Width + "  (H,W)" + "  BG Box = " + scaledBG.Height + "," + scaledBG.Width + "  (H,W)");

            //INIT PAWNS
            for (int i = 0; i < 8; i++)
            {
                WhitePieces[i] = new ChessPiece(ChessPiece.ChessNames.Pawn, ChessPiece.ChessColor.White,i);

            }
            
            //INIT Other Chesspieces
            WhitePieces[8] = new ChessPiece(ChessPiece.ChessNames.Rook, ChessPiece.ChessColor.White,0);
            WhitePieces[9] = new ChessPiece(ChessPiece.ChessNames.Knight, ChessPiece.ChessColor.White,0);
            WhitePieces[10] = new ChessPiece(ChessPiece.ChessNames.Bishop, ChessPiece.ChessColor.White,0);
            WhitePieces[11] = new ChessPiece(ChessPiece.ChessNames.King, ChessPiece.ChessColor.White,0);
            WhitePieces[12] = new ChessPiece(ChessPiece.ChessNames.Queen, ChessPiece.ChessColor.White,0);
            WhitePieces[13] = new ChessPiece(ChessPiece.ChessNames.Bishop, ChessPiece.ChessColor.White,1);
            WhitePieces[14] = new ChessPiece(ChessPiece.ChessNames.Knight, ChessPiece.ChessColor.White,1);
            WhitePieces[15] = new ChessPiece(ChessPiece.ChessNames.Rook, ChessPiece.ChessColor.White,1);

            //BlackPieces = (ChessPiece[]) WhitePieces.Clone();
            //Array.Copy(WhitePieces, BlackPieces, 16);
            for (int i = 0; i < 16; i++)
            {
                BlackPieces[i] = new ChessPiece(WhitePieces[i].chessName, ChessPiece.ChessColor.Black, WhitePieces[i].pieceID);
                BlackPieces[i].loadImage(BlackPieces[i].chessName, BlackPieces[i].chessColor);
            }

            //Calculate all locations (pixels)
            foreach (ChessPiece cp in WhitePieces)
            {
                cp.Location = convertChessPosToPix(cp.currentPosition);
            }
            foreach (ChessPiece cp in BlackPieces)
            {
                cp.Location = convertChessPosToPix(cp.currentPosition);
            }


            //Add 'Controls' to UI
            UI.Invoke(new Action(() => {

                UI.mainPanel.Click += MainPanel_Click;
                
                //add all pieces
                foreach (ChessPiece cp in WhitePieces)
                {
                    cp.generatePictureBox();
                    cp.Click += ControlBox_Click;
                    UI.mainPanel.Controls.Add(cp);
                }
                foreach (ChessPiece cp in BlackPieces)
                {
                    cp.generatePictureBox();
                    cp.Click += ControlBox_Click;
                    UI.mainPanel.Controls.Add(cp);
                }

                //Add BG and configure Window:
                UI.Size = new Size(scaledBG.Width, scaledBG.Height+yOffset);
                UI.mainPanel.BackgroundImage = scaledBG;
                UI.mainPanel.Location = new Point(0, 24);
                UI.mainPanel.Size = new Size(scaledBG.Width, scaledBG.Height);
                UI.mainPanel.SendToBack();

            }));

        }

        private void MainPanel_Click(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Right)
            {
                //Right mouse button is deselect all (for convienience)

            }
            else {
                Point coords = ((Panel)sender).PointToClient(Cursor.Position);

                //dividing coords by 70 (height and width of each field on the board) and removing decimals gives us the field in which was clicked

                int xclick = Convert.ToInt32(Math.Floor((double)(coords.X/70)))*70; // 
                int yclick = Convert.ToInt32(Math.Floor((double)(coords.Y/70)))*70;

                Point moveToPoint = new Point(xclick, yclick);

                //Console output for debug purposes
                // (8-yclick) to reverse the order (bottom row is 1, upper is 8, not otherway around)
                // (xclick+1) because far left is 1 (or A) not 0.
                Console.WriteLine("Mainpanel Field: " + convertNumberToLetter((xclick+1)/70) + ((8-yclick)/70) + "  clicked");
                //Check for selected chesspiece
                foreach (ChessPiece cp in WhitePieces)
                {
                    if (cp.selected == true) { moveChessPiece(cp, moveToPoint); cp.selected = false; cp.BorderStyle = BorderStyle.None; }
                }
                foreach (ChessPiece cp in BlackPieces)
                {
                    if (cp.selected == true) { moveChessPiece(cp, moveToPoint); cp.selected = false;  cp.BorderStyle = BorderStyle.None; }
                }
            }
        }

        private void moveChessPiece(ChessPiece cp, Point moveTo)
        {


            // Create a summary layout as an 2d array; To save increase easy of path calculation:
            
            bool moveLegit = false;

            int desiredMovementX = (cp.Location.X - moveTo.X) / 70;
            int desiredMovementY = (cp.Location.Y - moveTo.Y) / 70;

            //TODO: CHECK MOVE VALIDITY

            if(cp.chessName == ChessPiece.ChessNames.Pawn)
            {
                //Check Pawn-Movement Eligibility
                if(convertChessPosToPix(cp.defaultPosition) == cp.Location) {  // Pawn still in default position
                    // allowing 2 forward moves OR 1 diagonal move IF it strikes a chesspiece of the opposite color
                    if(cp.chessColor == ChessPiece.ChessColor.Black && (desiredMovementY==2 || desiredMovementY==1)) // 1-or-2 moves forward is legit if in default pos.
                    {
                        //check if path is clear:

                        checkClearPath(cp, moveTo);

                        moveLegit = true;
                    }
                    else if (cp.chessColor == ChessPiece.ChessColor.White && (desiredMovementY == -2 || desiredMovementY == -1)) // 1-or-2 moves forward is legit if in default pos.
                    {
                        //check if path is clear:



                        moveLegit = true;
                    }

                    else if(desiredMovementX/desiredMovementY==1 || desiredMovementX/desiredMovementY==-1)
                    {
                        // Diagonal 1 move.
                        // check if already a pawn there of opposite color.
                        if(cp.chessColor == ChessPiece.ChessColor.Black)
                        {
                            foreach (ChessPiece _cp in WhitePieces)
                            {
                                if (_cp.Location == moveTo) { moveLegit = true; }
                            }
                        }

                        else if (cp.chessColor == ChessPiece.ChessColor.White)
                        {
                            foreach (ChessPiece _cp in BlackPieces)
                            {
                                if (_cp.Location == moveTo) { moveLegit = true; }
                            }
                        }

                    }
                }
                else
                {
                    //allowing 1 forward move OR 1 diagonal move IF it strikes a chesspiece of the opposite color
                    if (cp.chessColor == ChessPiece.ChessColor.Black && (cp.Location.Y - moveTo.Y) <= 70 && (cp.Location.Y - moveTo.Y) >= 0) // 1-or-2 moves forward is legit if in default pos.
                    {
                        moveLegit = true;
                    }
                    else if (cp.chessColor == ChessPiece.ChessColor.White && (cp.Location.Y - moveTo.Y) >= -70 && (cp.Location.Y - moveTo.Y) <= 0) // 1-or-2 moves forward is legit if in default pos.
                    {
                        moveLegit = true;
                    }
                }
            }
            else if(cp.chessName == ChessPiece.ChessNames.Rook)
            {
                //Check Rook-Movement Eligibility
            }
            else if(cp.chessName == ChessPiece.ChessNames.Bishop)
            {
                //Check Bishop-Movement Eligibility
            }
            else if(cp.chessName == ChessPiece.ChessNames.Knight)
            {
                //Check Knight-Movement Eligibility
            }
            else if(cp.chessName == ChessPiece.ChessNames.Queen)
            {
                //Check Queen-Movement Eligibility
            }
            else if(cp.chessName == ChessPiece.ChessNames.King)
            {
                //Check King-Movement Eligibility
            }

            if (moveLegit)
            {
                //If moveTo == a point of an existing chesspiece (slag) remove the chesspiece  
                foreach (ChessPiece _cp in WhitePieces)
                {
                    if (moveTo == _cp.Location) { _cp.Location = new Point(1000, 1000); removeChessPiece(_cp); }
                }
                foreach (ChessPiece _cp in BlackPieces)
                {
                    if (moveTo == _cp.Location) { _cp.Location = new Point(1000, 1000); removeChessPiece(_cp); }
                }
                //MOVE CHESSPIECE
                Console.WriteLine("Moving chessPiece:" + cp.chessName + " " + cp.chessColor + " " + cp.pieceID + "  TO " + moveTo.ToString());
                cp.Location = new Point(moveTo.X, moveTo.Y);
            }
            else
            {
                Console.WriteLine("Incorrect move. Cancelling move.");
            }
        }
        private void ControlBox_Click(object sender, EventArgs e)
        {
            //Check all chesspieces if one is selected, if true -> deselect,  if not the same as selected now, select new
            bool deselect = false;

            //Console output for debug purposes
            Console.WriteLine("Chesspiece " + ((ChessPiece)sender).chessName +" "+ ((ChessPiece)sender).chessColor+ " with ID:" + ((ChessPiece)sender).pieceID + " was clicked.");


            foreach (ChessPiece cp in WhitePieces)
            {
                if(cp.chessName == ((ChessPiece)sender).chessName && cp.pieceID == ((ChessPiece)sender).pieceID && cp.selected == true) { deselect = true; } // check if clicked one is the same as already selected
                if(cp.selected == true) {
                    //If a chesspiece is already selected:
                    if (((ChessPiece)sender).chessColor == ChessPiece.ChessColor.Black)
                    {
                        //Whtie selected, black clicked (perform move.)
                        moveChessPiece(cp, ((ChessPiece)sender).Location);
                        deselect = true;
                    }
                    cp.selected = false;
                    cp.BorderStyle = BorderStyle.None;
                    
                } // if not, deselect it
            }

            foreach (ChessPiece cp in BlackPieces)
            {
                if (cp.chessName == ((ChessPiece)sender).chessName && cp.pieceID == ((ChessPiece)sender).pieceID && cp.selected == true) { deselect = true; } // check if clicked one is the same as already selected
                if (cp.selected == true) {
                    //If a chesspiece is already selected:
                    if(((ChessPiece)sender).chessColor == ChessPiece.ChessColor.White)
                    {
                        //Black selected, white clicked (perform move.)
                        moveChessPiece(cp, ((ChessPiece)sender).Location);
                        deselect = true;
                    }
                    cp.selected = false;
                    cp.BorderStyle = BorderStyle.None;
                } // if not, deselect it
            }

            if (!deselect) // If the object clicked is not the same as the one selected then draw a border
            {
                ((ChessPiece)sender).selected = true;
                ((ChessPiece)sender).BorderStyle = BorderStyle.FixedSingle;
            }

        }

        public Bitmap scaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            Bitmap bmp = new Bitmap(newImage);

            return bmp;
        }

        public Point convertChessPosToPix(string chessPos)
        {
            //Calculate Position ( A * B ) (Where board = 8*8)

            int xPosPix = 0;
            switch (chessPos.Substring(0,1))
            {
                case "A":
                    xPosPix = 0;
                    break;
                case "B":
                    xPosPix = 1;
                    break;
                case "C":
                    xPosPix = 2;
                    break;
                case "D":
                    xPosPix = 3;
                    break;
                case "E":
                    xPosPix = 4;
                    break;
                case "F":
                    xPosPix = 5;
                    break;
                case "G":
                    xPosPix = 6;
                    break;
                case "H":
                    xPosPix = 7;
                    break;
                default:
                    break;
            }
            int yPosPix = Convert.ToInt32(chessPos.Substring(1, 1));
            
            //Convert Position A,B to pixels (*35)    
            return new Point(xPosPix * 70, (yPosPix-1) * 70);
        }

        public string convertNumberToLetter(int number)
        {
            switch (number)
            {
                case 1:
                    return "A";
                case 2:
                    return "B";
                case 3:
                    return "C";
                case 4:
                    return "D";
                case 5:
                    return "E";
                case 6:
                    return "F";
                case 7:
                    return "G";
                default:
                    return "Error";
            }
        }

        public void removeChessPiece(ChessPiece cp)
        {
            cp.alive = false;
            Console.WriteLine("Removing chessPiece:" + cp.chessName + " " + cp.chessColor + " " + cp.pieceID);
            UI.Invoke(new Action(() =>
            {
                UI.mainPanel.Controls.Remove(cp);
            }));
        }

        public int[,] getChessBoardArray()
        {
            int[,] chessBoard = new int[8,8];

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    chessBoard[x,y] = 0;
                }
            }

            // fill array -> 0 = EMPTY ; 1 WHITE ; 2 BLACK
            foreach (ChessPiece cp in WhitePieces)
            {
                if (cp.alive)
                { 
                    chessBoard[cp.Location.X / 70, cp.Location.Y / 70] = 1;
                }
            }
            foreach (ChessPiece cp in BlackPieces)
            {
                if (cp.alive)
                {
                    chessBoard[cp.Location.X / 70, cp.Location.Y / 70] = 2;
                }
            }

            return chessBoard;
        }

        public bool checkClearPath(ChessPiece cp, Point moveTo)
        {
            int[,] chessBoardSummary = getChessBoardArray();

            int desiredMovesX = (cp.Location.X - moveTo.X)/70;
            int desiredMovesY = (cp.Location.Y - moveTo.Y)/70;

            int locX = cp.Location.X / 70;
            int locY = cp.Location.Y / 70;


            bool clearPath = true;

            if (desiredMovesX == 0)
            {
                //Check Vertical
                for (int i = 0; i < Math.Abs(desiredMovesY); i++)
                {
                    Console.WriteLine("LocX:"+locX + " LocY:"+(8-locY+i)+ " --- "+chessBoardSummary[locX, (8 - locY + i)]);
                    if (!(chessBoardSummary[locX, (8 - locY + i)] == 0)) { clearPath = false; }
                }
            }
            else if(desiredMovesY == 0)
            {
                //Check Horizontal
                for (int i = 0; i < Math.Abs(desiredMovesX); i++)
                {
                    if(!(chessBoardSummary[locX+i, (8 - locY)] == 0)) { clearPath = false; }
                }

            }
            else if(desiredMovesX==desiredMovesY)
            {
                //Check Diagonal
            }

            return clearPath;
        }
    }
}
