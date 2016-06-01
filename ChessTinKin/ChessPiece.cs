using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessTinKin
{
    public class ChessPiece : PictureBox
    {
        public enum ChessNames{ Bishop, Rook, Pawn, Knight, Queen, King };
        public enum ChessColor { Black, White };
        

        public ChessColor chessColor { get; set; }
        public ChessNames chessName { get; set; }

        public int pieceID { get; set; }

        public string currentPosition { get; set; }

        public bool selected = false;

        Image image { get; set; }

        public int widthField = 70; // 600/16=37.5
        public int heightField = 70;

        //public PictureBox controlBox { get; set; }

        public ChessPiece(ChessNames c, ChessColor color, int id)
        {
            chessColor = color;
            chessName = c;
            pieceID = id;

            loadImage(c, color);
            currentPosition = getDefaultPosition();
            generatePictureBox();

            
           
        }

        public void loadImage(ChessNames c, ChessColor color)
        {

            switch (c)
            {
                case ChessNames.Bishop:
                    image = Image.FromFile("Pictures/Bishop" + color + ".png");
                    break;
                case ChessNames.Rook:
                    image = Image.FromFile("Pictures/Rook" + color + ".png");
                    break;
                case ChessNames.Pawn:
                    image = Image.FromFile("Pictures/Pawn" + color + ".png");
                    break;
                case ChessNames.Knight:
                    image = Image.FromFile("Pictures/Knight" + color + ".png");
                    break;
                case ChessNames.Queen:
                    image = Image.FromFile("Pictures/Queen" + color + ".png");
                    break;
                case ChessNames.King:
                    image = Image.FromFile("Pictures/King" + color + ".png");
                    break;
                default:
                    break;
            }

            
        }
        public string getDefaultPosition()
        {
            string position = "";

            if(chessColor == ChessColor.White)
            {
                switch (chessName)
                {
                    case ChessNames.Bishop:
                        if (pieceID == 0) position = "C1";
                        if (pieceID == 1) position = "F1";
                        break;
                    case ChessNames.Rook:
                        if (pieceID == 0) position = "A1";
                        if (pieceID == 1) position = "H1";
                        break;
                    case ChessNames.Pawn:
                        if (pieceID == 0) position = "A2";
                        if (pieceID == 1) position = "B2";
                        if (pieceID == 2) position = "C2";
                        if (pieceID == 3) position = "D2";
                        if (pieceID == 4) position = "E2";
                        if (pieceID == 5) position = "F2";
                        if (pieceID == 6) position = "G2";
                        if (pieceID == 7) position = "H2";
                        break;
                    case ChessNames.Knight:
                        if (pieceID == 0) position = "B1";
                        if (pieceID == 1) position = "G1";
                        break;
                    case ChessNames.Queen:
                        position = "E1";
                        break;
                    case ChessNames.King:
                        position = "D1";
                        break;
                    default:
                        break;
                }
            }

            else if (chessColor == ChessColor.Black)
            {
                switch (chessName)
                {
                    case ChessNames.Bishop:
                        if (pieceID == 0) position = "C8";
                        if (pieceID == 1) position = "F8";
                        break;
                    case ChessNames.Rook:
                        if (pieceID == 0) position = "A8";
                        if (pieceID == 1) position = "H8";
                        break;
                    case ChessNames.Pawn:
                        if (pieceID == 0) position = "A7";
                        if (pieceID == 1) position = "B7";
                        if (pieceID == 2) position = "C7";
                        if (pieceID == 3) position = "D7";
                        if (pieceID == 4) position = "E7";
                        if (pieceID == 5) position = "F7";
                        if (pieceID == 6) position = "G7";
                        if (pieceID == 7) position = "H7";
                        break;
                    case ChessNames.Knight:
                        if (pieceID == 0) position = "B8";
                        if (pieceID == 1) position = "G8";
                        break;
                    case ChessNames.Queen:
                        position = "E8";
                        break;
                    case ChessNames.King:
                        position = "D8";
                        break;
                    default:
                        break;
                }
            }
            return position;
        }

        public void generatePictureBox()
        {
            //controlBox = new PictureBox();
            this.Image = image;
            this.Height = heightField;
            this.Width = widthField;
            this.SizeMode = PictureBoxSizeMode.StretchImage;
            this.BackColor = Color.Transparent;
            this.BorderStyle = BorderStyle.None;
        }

      
    }
}
