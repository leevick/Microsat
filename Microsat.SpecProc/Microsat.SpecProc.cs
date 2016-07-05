using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsat.SpecProc
{
    public class colourSystem
    {
        public string name;
        public double xRed, yRed,              /* Red x, y */
        xGreen, yGreen,         /* Green x, y */
        xBlue, yBlue,           /* Blue x, y */
        xWhite, yWhite,         /* White point x, y */
        gamma;                  /* Gamma correction for system */
        public colourSystem(string n, double xr, double yr, double xg, double yg, double xb, double yb, double xw, double yw, double g)
        {
            this.name = n;
            this.xRed = xr;
            this.yRed = yr;
            this.xGreen = xg;
            this.yGreen = yg;
            this.xBlue = xb;
            this.yBlue = yb;
            this.xWhite = xw;
            this.yWhite = yw;
            this.gamma = g;

        }
    };
    public class Spectra2RGB
    {
        public double[,] Map;
        public double t = 0, x = 0, y = 0, z = 0, r = 0, g = 0, b = 0;
        public double[,] cie_colour_match = new double[81, 3]{
        { 0.0014,0.0000,0.0065 },{ 0.0022,0.0001,0.0105 },{ 0.0042,0.0001,0.0201 },
        { 0.0076,0.0002,0.0362 },{ 0.0143,0.0004,0.0679 },{ 0.0232,0.0006,0.1102 },
        { 0.0435,0.0012,0.2074 },{ 0.0776,0.0022,0.3713 },{ 0.1344,0.0040,0.6456 },
        { 0.2148,0.0073,1.0391 },{ 0.2839,0.0116,1.3856 },{ 0.3285,0.0168,1.6230 },
        { 0.3483,0.0230,1.7471 },{ 0.3481,0.0298,1.7826 },{ 0.3362,0.0380,1.7721 },
        { 0.3187,0.0480,1.7441 },{ 0.2908,0.0600,1.6692 },{ 0.2511,0.0739,1.5281 },
        { 0.1954,0.0910,1.2876 },{ 0.1421,0.1126,1.0419 },{ 0.0956,0.1390,0.8130 },
        { 0.0580,0.1693,0.6162 },{ 0.0320,0.2080,0.4652 },{ 0.0147,0.2586,0.3533 },
        { 0.0049,0.3230,0.2720 },{ 0.0024,0.4073,0.2123 },{ 0.0093,0.5030,0.1582 },
        { 0.0291,0.6082,0.1117 },{ 0.0633,0.7100,0.0782 },{ 0.1096,0.7932,0.0573 },
        { 0.1655,0.8620,0.0422 },{ 0.2257,0.9149,0.0298 },{ 0.2904,0.9540,0.0203 },
        { 0.3597,0.9803,0.0134 },{ 0.4334,0.9950,0.0087 },{ 0.5121,1.0000,0.0057 },
        { 0.5945,0.9950,0.0039 },{ 0.6784,0.9786,0.0027 },{ 0.7621,0.9520,0.0021 },
        { 0.8425,0.9154,0.0018 },{ 0.9163,0.8700,0.0017 },{ 0.9786,0.8163,0.0014 },
        { 1.0263,0.7570,0.0011 },{ 1.0567,0.6949,0.0010 },{ 1.0622,0.6310,0.0008 },
        { 1.0456,0.5668,0.0006 },{ 1.0026,0.5030,0.0003 },{ 0.9384,0.4412,0.0002 },
        { 0.8544,0.3810,0.0002 },{ 0.7514,0.3210,0.0001 },{ 0.6424,0.2650,0.0000 },
        { 0.5419,0.2170,0.0000 },{ 0.4479,0.1750,0.0000 },{ 0.3608,0.1382,0.0000 },
        { 0.2835,0.1070,0.0000 },{ 0.2187,0.0816,0.0000 },{ 0.1649,0.0610,0.0000 },
        { 0.1212,0.0446,0.0000 },{ 0.0874,0.0320,0.0000 },{ 0.0636,0.0232,0.0000 },
        { 0.0468,0.0170,0.0000 },{ 0.0329,0.0119,0.0000 },{ 0.0227,0.0082,0.0000 },
        { 0.0158,0.0057,0.0000 },{ 0.0114,0.0041,0.0000 },{ 0.0081,0.0029,0.0000 },
        { 0.0058,0.0021,0.0000 },{ 0.0041,0.0015,0.0000 },{ 0.0029,0.0010,0.0000 },
        { 0.0020,0.0007,0.0000 },{ 0.0014,0.0005,0.0000 },{ 0.0010,0.0004,0.0000 },
        { 0.0007,0.0002,0.0000 },{ 0.0005,0.0002,0.0000 },{ 0.0003,0.0001,0.0000 },
        { 0.0002,0.0001,0.0000 },{ 0.0002,0.0001,0.0000 },{ 0.0001,0.0000,0.0000 },
        { 0.0001,0.0000,0.0000 },{ 0.0001,0.0000,0.0000 },{ 0.0000,0.0000,0.0000 }
    };
        public double[,] spectra = new double[160, 2];
        public colourSystem[] colorsys = new colourSystem[6] { new colourSystem("NTSC", 0.67, 0.33, 0.21, 0.71, 0.14, 0.08, 0.3101, 0.3162, 0), new colourSystem("EBU (PAL/SECAM)", 0.64, 0.33, 0.29, 0.60, 0.15, 0.06, 0.3127, 0.3291, 0), new colourSystem("SMPTE", 0.630, 0.340, 0.310, 0.595, 0.155, 0.070, 0.3127, 0.3291, 0), new colourSystem("HDTV", 0.670, 0.330, 0.210, 0.710, 0.150, 0.060, 0.3127, 0.3291, 0), new colourSystem("CIE", 0.7355, 0.2645, 0.2658, 0.7243, 0.1669, 0.0085, 0.33333333, 0.33333333, 0), new colourSystem("CIE REC 709", 0.64, 0.33, 0.30, 0.60, 0.15, 0.06, 0.3127, 0.3291, 0) };


        public Spectra2RGB(double[,] spec)
        {
            this.spectra = spec;
        }

        public void spectrum_to_xyz()
        {


            double X = 0, Y = 0, Z = 0, XYZ;

            /* CIE colour matching functions xBar, yBar, and zBar for
            wavelengths from 380 through 780 nanometers, every 5
            nanometers.  For a wavelength lambda in this range:

            cie_colour_match[(lambda - 380) / 5][0] = xBar
            cie_colour_match[(lambda - 380) / 5][1] = yBar
            cie_colour_match[(lambda - 380) / 5][2] = zBar

            To save memory, this table can be declared as floats
            rather than doubles; (IEEE) float has enough
            significant bits to represent the values. It's declared
            as a double here to avoid warnings about "conversion
            between floating-point types" from certain persnickety
            compilers. */
            /*for (int i = 0, lambda = 380; lambda< 780.1; i++, lambda += 5) {
		        double Me;

                Me = spec_intens(lambda);
		        X += Me* cie_colour_match[i,0];
		        Y += Me* cie_colour_match[i,1];
		        Z += Me* cie_colour_match[i,2];
	        }
            */

            double[,] temp = new double[81, 2];

            for (int i = 0; i < 160; i++)
            {
                int j = (int)((spectra[i, 1] - 377.5) / 5);

                if (j >= 0 && j <= 80)
                {

                    temp[j, 0] += spectra[i, 0];


                    temp[j, 1]++;

                }

            }


            for (int i = 0; i < 81; i++)
            {



                if (temp[i, 1] != 0)
                {
                    double Me = temp[i, 0] / temp[i, 1];
                    X += Me * cie_colour_match[i, 0];
                    Y += Me * cie_colour_match[i, 1];
                    Z += Me * cie_colour_match[i, 2];

                }

            }


            XYZ = (X + Y + Z);
            this.x = X / XYZ;
            this.y = Y / XYZ;
            this.z = Z / XYZ;
        }

        public void xyz_to_rgb(colourSystem cs, double xc, double yc, double zc)
        {

            double xr, yr, zr, xg, yg, zg, xb, yb, zb;
            double xw, yw, zw;
            double rx, ry, rz, gx, gy, gz, bx, by, bz;
            double rw, gw, bw;

            xr = cs.xRed; yr = cs.yRed; zr = 1 - (xr + yr);
            xg = cs.xGreen; yg = cs.yGreen; zg = 1 - (xg + yg);
            xb = cs.xBlue; yb = cs.yBlue; zb = 1 - (xb + yb);

            xw = cs.xWhite; yw = cs.yWhite; zw = 1 - (xw + yw);

            /* xyz -> rgb matrix, before scaling to white. */

            rx = (yg * zb) - (yb * zg); ry = (xb * zg) - (xg * zb); rz = (xg * yb) - (xb * yg);
            gx = (yb * zr) - (yr * zb); gy = (xr * zb) - (xb * zr); gz = (xb * yr) - (xr * yb);
            bx = (yr * zg) - (yg * zr); by = (xg * zr) - (xr * zg); bz = (xr * yg) - (xg * yr);

            /* White scaling factors.
            Dividing by yw scales the white luminance to unity, as conventional. */

            rw = ((rx * xw) + (ry * yw) + (rz * zw)) / yw;
            gw = ((gx * xw) + (gy * yw) + (gz * zw)) / yw;
            bw = ((bx * xw) + (by * yw) + (bz * zw)) / yw;

            /* xyz -> rgb matrix, correctly scaled to white. */

            rx = rx / rw; ry = ry / rw; rz = rz / rw;
            gx = gx / gw; gy = gy / gw; gz = gz / gw;
            bx = bx / bw; by = by / bw; bz = bz / bw;

            /* rgb of the desired point */

            this.r = (rx * xc) + (ry * yc) + (rz * zc);
            this.g = (gx * xc) + (gy * yc) + (gz * zc);
            this.b = (bx * xc) + (by * yc) + (bz * zc);
        }


        public void Conversion()
        {
            spectrum_to_xyz();
            xyz_to_rgb(colorsys[2], x, y, z);
        }







        /// <summary>
        /// Convert HSV to RGB
        /// h is from 0-360
        /// s,v values are 0-1
        /// r,g,b values are 0-255
        /// Based upon http://ilab.usc.edu/wiki/index.php/HSV_And_H2SV_Color_Space#HSV_Transformation_C_.2F_C.2B.2B_Code_2
        /// </summary>
        public static void HsvToRgb(double h, double S, double V, out byte r, out byte g, out byte b)
        {
            // ######################################################################
            // T. Nathan Mundhenk
            // mundhenk@usc.edu
            // C/C++ Macro HSV to RGB

            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            r = Clamp((int)(R * 255.0));
            g = Clamp((int)(G * 255.0));
            b = Clamp((int)(B * 255.0));
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        public static byte Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return (byte)i;
        }


    }
}
