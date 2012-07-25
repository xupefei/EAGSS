// BitmapFont.cs
// Bitmap Font class for XNA
// Copyright 2006 Microsoft Corp.
// Revision: 2006-Aug-30

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EAGSS
{
    /// <summary>
    /// Info for each glyph in the font - where to find the glyph image and
    /// other properties
    /// </summary>
    internal struct GlyphInfo
    {
        public int nAdvanceWidth;
        public int nBitmapID;
        public int nHeight;
        public int nLeftSideBearing;
        public int nOriginX;
        public int nOriginY;
        public int nWidth;
    }

    /// <summary>
    /// Info for each font bitmap
    /// </summary>
    internal struct BitmapInfo
    {
        public int nX, nY;
        public string strFilename;
    }

    /// <summary>
    /// Bitmap font class for XNA
    /// </summary>
    public class BitmapFont
    {
        private readonly ContentLoader content;
        private readonly Dictionary<int, BitmapInfo> m_dictBitmapID2BitmapInfo;
        private readonly Dictionary<int, Texture2D> m_dictBitmapID2Texture;
        private readonly Dictionary<char, Dictionary<char, int>> m_dictKern;
        private readonly Dictionary<char, GlyphInfo> m_dictUnicode2GlyphInfo;
        private int _mNBase;
        private int _mNHeight;
        private SpriteBatch mSb;

        /// <summary>
        /// Current color used for drawing text
        /// </summary>
        private Color m_color = Color.White;

        /// <summary>
        /// Should we kern adjacent characters?
        /// </summary>
        private bool m_fKern = true;

        /// <summary>
        /// Current pen position
        /// </summary>
        private Vector2 m_vPen = new Vector2(0, 0);

        /// <summary>
        /// Create a new font from the info in the specified font descriptor (XML) file
        /// </summary>
        /// <param name="game">current game</param>
        /// <param name="strFontFilename">font file (.xml)</param>
        public BitmapFont(EAGSS game, string strFontFilename)
        {
            content = game.Content;

            m_dictBitmapID2BitmapInfo = new Dictionary<int, BitmapInfo>();
            m_dictBitmapID2Texture = new Dictionary<int, Texture2D>();

            m_dictUnicode2GlyphInfo = new Dictionary<char, GlyphInfo>();
            m_dictKern = new Dictionary<char, Dictionary<char, int>>();

            var xd = new XmlDocument();
            xd.Load(new MemoryStream(content.Load<byte[]>(strFontFilename)));
            LoadFontXML(xd.ChildNodes);
        }

        /// <summary>
        /// Enable/disable kerning
        /// </summary>
        public bool KernEnable
        {
            get { return m_fKern; }
            set { m_fKern = value; }
        }

        /// <summary>
        /// Distance from top of font to the baseline
        /// </summary>
        public int Baseline
        {
            get { return _mNBase; }
        }

        /// <summary>
        /// Distance from top to bottom of the font
        /// </summary>
        public int LineHeight
        {
            get { return _mNHeight; }
        }

        /// <summary>
        /// Current pen position
        /// </summary>
        public Vector2 Pen
        {
            get { return m_vPen; }
            set { m_vPen = value; }
        }

        /// <summary>
        /// Current color used for drawing text
        /// </summary>
        public Color TextColor
        {
            get { return m_color; }
            set { m_color = value; }
        }

        /// <summary>
        /// Reset the font when the device has changed
        /// </summary>
        /// <param name="device">The new device</param>
        public void Reset(GraphicsDevice device)
        {
            mSb = new SpriteBatch(device);

            foreach (var kv in m_dictBitmapID2BitmapInfo)
                m_dictBitmapID2Texture[kv.Key] = content.Load<Texture2D>("fonts\\" + kv.Value.strFilename);
        }

        /// <summary>
        /// Calculate the width of the given string.
        /// </summary>
        /// <param name="format">String format</param>
        /// <param name="args">String format arguments</param>
        /// <returns>Width and height of the string</returns>
        public Vector2 MeasureString(string format, params object[] args)
        {
            string str = string.Format(format, args);
            char cLast = '\0';

            int stringHeight = 0;

            int maxWidth = 0;
            foreach (string line in str.Split('\n'))
            {
                int nWidth = 0;

                foreach (char c in line)
                {
                    if (!m_dictUnicode2GlyphInfo.ContainsKey(c))
                    {
                        //TODO: print out undefined char glyph
                        continue;
                    }

                    GlyphInfo ginfo = m_dictUnicode2GlyphInfo[c];

                    // if kerning is enabled, get the kern adjustment for this char pair
                    if (m_fKern)
                    {
                        nWidth += CalcKern(cLast, c);
                        cLast = c;
                    }

                    // update the string width
                    nWidth += ginfo.nAdvanceWidth;
                }

                maxWidth = (nWidth > maxWidth) ? nWidth : maxWidth;
                stringHeight += _mNHeight;
            }

            return new Vector2(maxWidth, stringHeight);
        }

        /// <summary>
        /// Set the current pen position
        /// </summary>
        /// <param name="x">X-coord</param>
        /// <param name="y">Y-coord</param>
        public void SetPen(int x, int y)
        {
            m_vPen = new Vector2(x, y);
        }

        /// <summary>
        /// Draw the given string at (x,y).
        /// The text color is inherited from the last draw command (default=White).
        /// </summary>
        /// <param name="x">X-coord</param>
        /// <param name="y">Y-coord</param>
        /// <param name="format">String format</param>
        /// <param name="args">String format args</param>
        /// <returns>Width of string (in pixels)</returns>
        public int DrawString(int x, int y, string format, params object[] args)
        {
            var v = new Vector2(x, y);
            return DrawString(v, m_color, format, args);
        }

        /// <summary>
        /// Draw the given string at (x,y) using the specified color
        /// </summary>
        /// <param name="x">X-coord</param>
        /// <param name="y">Y-coord</param>
        /// <param name="color">Text color</param>
        /// <param name="format">String format</param>
        /// <param name="args">String format args</param>
        /// <returns>Width of string (in pixels)</returns>
        public int DrawString(int x, int y, Color color, string format, params object[] args)
        {
            var v = new Vector2(x, y);
            return DrawString(v, color, format, args);
        }

        /// <summary>
        /// Draw the given string using the specified color.
        /// The text drawing location is immediately after the last drawn text (default=0,0).
        /// </summary>
        /// <param name="color">Text color</param>
        /// <param name="format">String format</param>
        /// <param name="args">String format args</param>
        /// <returns>Width of string (in pixels)</returns>
        public int DrawString(Color color, string format, params object[] args)
        {
            return DrawString(m_vPen, color, format, args);
        }

        /// <summary>
        /// Draw the given string at (x,y).
        /// The text drawing location is immediately after the last drawn text (default=0,0).
        /// The text color is inherited from the last draw command (default=White).
        /// </summary>
        /// <param name="format">String format</param>
        /// <param name="args">String format args</param>
        /// <returns>Width of string (in pixels)</returns>
        public int DrawString(string format, params object[] args)
        {
            return DrawString(m_vPen, m_color, format, args);
        }

        /// <summary>
        /// Draw the given string at vOrigin using the specified color
        /// </summary>
        /// <param name="vOrigin">(x,y) coord</param>
        /// <param name="color">Text color</param>
        /// <param name="format">String format</param>
        /// <param name="args">String format args</param>
        /// <returns>Width of string (in pixels)</returns>
        public int DrawString(Vector2 vOrigin, Color color, string format, params object[] args)
        {
            string str = string.Format(format, args);

            Vector2 vAt = vOrigin;
            int nWidth = 0;
            char cLast = '\0';

            mSb.Begin();

            // draw each character in the string
            foreach (char c in str)
            {
                if (c == '\n')
                {
                    vAt.X = vOrigin.X;
                    vAt.Y += _mNHeight;
                }
                if (!m_dictUnicode2GlyphInfo.ContainsKey(c))
                {
                    //TODO: print out undefined char glyph
                    continue;
                }

                GlyphInfo ginfo = m_dictUnicode2GlyphInfo[c];

                // if kerning is enabled, get the kern adjustment for this char pair
                if (m_fKern)
                {
                    int nKern = CalcKern(cLast, c);
                    vAt.X += nKern;
                    nWidth += nKern;
                    cLast = c;
                }

                // draw the glyph
                vAt.X += ginfo.nLeftSideBearing;
                if (ginfo.nWidth != 0 && ginfo.nHeight != 0)
                {
                    var r = new Rectangle(ginfo.nOriginX, ginfo.nOriginY, ginfo.nWidth, ginfo.nHeight);
                    mSb.Draw(m_dictBitmapID2Texture[ginfo.nBitmapID], vAt, r, color);
                }

                // update the string width and advance the pen to the next drawing position
                nWidth += ginfo.nAdvanceWidth;
                vAt.X += ginfo.nAdvanceWidth - ginfo.nLeftSideBearing;
            }

            mSb.End();

            // record final pen position and color
            m_vPen = vAt;
            m_color = color;

            return nWidth;
        }

        /// <summary>
        /// Get the kern value for the given pair of characters
        /// </summary>
        /// <param name="chLeft">Left character</param>
        /// <param name="chRight">Right character</param>
        /// <returns>Amount to kern (in pixels)</returns>
        private int CalcKern(char chLeft, char chRight)
        {
            if (m_dictKern.ContainsKey(chLeft))
            {
                Dictionary<char, int> kern2 = m_dictKern[chLeft];
                if (kern2.ContainsKey(chRight))
                    return kern2[chRight];
            }
            return 0;
        }

        #region Load Font from XML

        /// <summary>
        /// Load the font data from an XML font descriptor file
        /// </summary>
        /// <param name="xnl">XML node list containing the entire font descriptor file</param>
        private void LoadFontXML(XmlNodeList xnl)
        {
            foreach (XmlNode xn in xnl)
            {
                if (xn.Name == "font")
                {
                    _mNBase = Int32.Parse(GetXMLAttribute(xn, "base"));
                    _mNHeight = Int32.Parse(GetXMLAttribute(xn, "height"));

                    LoadFontXML_font(xn.ChildNodes);
                }
            }
        }

        /// <summary>
        /// Load the data from the "font" node
        /// </summary>
        /// <param name="xnl">XML node list containing the "font" node's children</param>
        private void LoadFontXML_font(XmlNodeList xnl)
        {
            foreach (XmlNode xn in xnl)
            {
                if (xn.Name == "bitmaps")
                    LoadFontXML_bitmaps(xn.ChildNodes);
                if (xn.Name == "glyphs")
                    LoadFontXML_glyphs(xn.ChildNodes);
                if (xn.Name == "kernpairs")
                    LoadFontXML_kernpairs(xn.ChildNodes);
            }
        }

        /// <summary>
        /// Load the data from the "bitmaps" node
        /// </summary>
        /// <param name="xnl">XML node list containing the "bitmaps" node's children</param>
        private void LoadFontXML_bitmaps(XmlNodeList xnl)
        {
            foreach (XmlNode xn in xnl)
            {
                if (xn.Name == "bitmap")
                {
                    string strID = GetXMLAttribute(xn, "id");
                    string strFilename = GetXMLAttribute(xn, "name");
                    string strSize = GetXMLAttribute(xn, "size");
                    string[] aSize = strSize.Split('x');

                    BitmapInfo bminfo;
                    bminfo.strFilename = strFilename;
                    bminfo.nX = Int32.Parse(aSize[0]);
                    bminfo.nY = Int32.Parse(aSize[1]);

                    m_dictBitmapID2BitmapInfo[Int32.Parse(strID)] = bminfo;
                }
            }
        }

        /// <summary>
        /// Load the data from the "glyphs" node
        /// </summary>
        /// <param name="xnl">XML node list containing the "glyphs" node's children</param>
        private void LoadFontXML_glyphs(XmlNodeList xnl)
        {
            foreach (XmlNode xn in xnl)
            {
                if (xn.Name == "glyph")
                {
                    string strChar = GetXMLAttribute(xn, "ch");
                    string strBitmapID = GetXMLAttribute(xn, "bm");
                    string strOrigin = GetXMLAttribute(xn, "origin");
                    string strSize = GetXMLAttribute(xn, "size");
                    string strAW = GetXMLAttribute(xn, "aw");
                    string strLSB = GetXMLAttribute(xn, "lsb");

                    string[] aOrigin = strOrigin.Split(',');
                    string[] aSize = strSize.Split('x');

                    var ginfo = new GlyphInfo();
                    ginfo.nBitmapID = Int32.Parse(strBitmapID);
                    ginfo.nOriginX = Int32.Parse(aOrigin[0]);
                    ginfo.nOriginY = Int32.Parse(aOrigin[1]);
                    ginfo.nWidth = Int32.Parse(aSize[0]);
                    ginfo.nHeight = Int32.Parse(aSize[1]);
                    ginfo.nAdvanceWidth = Int32.Parse(strAW);
                    ginfo.nLeftSideBearing = Int32.Parse(strLSB);

                    m_dictUnicode2GlyphInfo[strChar[0]] = ginfo;
                }
            }
        }

        /// <summary>
        /// Load the data from the "kernpairs" node
        /// </summary>
        /// <param name="xnl">XML node list containing the "kernpairs" node's children</param>
        private void LoadFontXML_kernpairs(XmlNodeList xnl)
        {
            foreach (XmlNode xn in xnl)
            {
                if (xn.Name == "kernpair")
                {
                    string strLeft = GetXMLAttribute(xn, "left");
                    string strRight = GetXMLAttribute(xn, "right");
                    string strAdjust = GetXMLAttribute(xn, "adjust");

                    char chLeft = strLeft[0];
                    char chRight = strRight[0];

                    // create a kern dict for the left char if needed
                    if (!m_dictKern.ContainsKey(chLeft))
                        m_dictKern[chLeft] = new Dictionary<char, int>();

                    // add the right char to the left char's kern dict
                    Dictionary<char, int> kern2 = m_dictKern[chLeft];
                    kern2[chRight] = Int32.Parse(strAdjust);
                }
            }
        }

        /// <summary>
        /// Get the XML attribute value (without throwing if the attribute doesn't exist)
        /// </summary>
        /// <param name="n">XML node</param>
        /// <param name="strAttr">Attribute name</param>
        /// <returns>Attribute value, or the empty string if the attribute doesn't exist</returns>
        private static string GetXMLAttribute(XmlNode n, string strAttr)
        {
            string strVal = "";
            try
            {
                strVal = n.Attributes[strAttr].Value;
            }
            catch
            {
                strVal = "";
            }
            return strVal;
        }

        #endregion Load Font from XML
    }
}