using PinMame;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace PinEmu
{
	public partial class Form1 : Form
	{
		private static PinMame.PinMame _pinMame;
		private static string[] _dmdMap;
		private static bool _isRunning = false;
		private static bool _started = false;
		private static int count = 0;
		private static Dictionary<byte, byte> _dmdLevels;
		
		public Form1()
		{
			InitializeComponent();
			AllocConsole();

			//_pinMame.OnConsoleDataUpdated += _pinMame_OnConsoleDataUpdated;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool AllocConsole();

		private void _pinMame_OnConsoleDataUpdated(nint dataPtr, int size)
		{
			throw new NotImplementedException();
		}

		private void _pinMame_OnGameEnded()
		{
			//Game is Ended
			//throw new NotImplementedException();
			Debug.WriteLine("OnGameEnded");
		}

		private void _pinMame_OnGameStarted()
		{
			//throw new NotImplementedException();
			Debug.WriteLine("OnGameStarted");
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				_pinMame = PinMame.PinMame.Instance(vpmPath:"Z:\\Dev\\VPinMAME36_Minimal");// vpmPath:"Z:\\Dev\\pinmame\\vcproj\\obj\\VC2022\\VPinMAME\\x64\\Debug");
				_pinMame.OnGameStarted += _pinMame_OnGameStarted;
				_pinMame.OnGameEnded += _pinMame_OnGameEnded;
				_pinMame.OnDisplayAvailable += _pinMame_OnDisplayAvailable;
				_pinMame.OnDisplayUpdated += _pinMame_OnDisplayUpdated;
				_pinMame.ApplyConfig();
				
				
			}
			catch { }
		}

		private void _pinMame_OnDisplayUpdated(int index, nint framePtr, PinMameDisplayLayout displayLayout)
		{
			if (displayLayout.IsDmd && _dmdMap != null)
			{
				DumpDmd(this, index, framePtr, displayLayout);
			}
			else
			{
				//DumpAlpha(index, framePtr, displayLayout);
			}
		}

		private void _pinMame_OnDisplayAvailable(int index, int displayCount, PinMameDisplayLayout displayLayout)
		{
			if (displayLayout.IsDmd)
			{
				//if (displayLayout.Depth == 2)
				//{
				//	_dmdMap = new[] {
				//		"░", "▒", "▓", "▓"
				//	};

				//}
				//else
				//{
					_dmdMap = new[] {
						"░", "░", "░", "░",
						"▒", "▒", "▒", "▒",
						"▓", "▓", "▓", "▓",
						"▓", "▓", "▓", "▓",
					};
				//}
			}
			_dmdLevels = displayLayout.Levels;
		}

		static unsafe void DumpDmd(Form1 frm, int index, IntPtr framePtr, PinMameDisplayLayout displayLayout)
		{
			byte* ptr = (byte*)framePtr;
			byte[] raw = new byte[displayLayout.Width * displayLayout.Height];
			for(int i = 0; i< raw.Length; i++)
			{
				raw[i] = ptr[i];
			}

			frm.dmd.Image = RawBytesToBitmap(raw, displayLayout.Width, displayLayout.Height);

			return;


			for (var y = 0; y < displayLayout.Height; y++)
			{
				var dmd = "";

				for (var x = 0; x < displayLayout.Width; x++)
				{
					//dmd += _dmdMap[_dmdLevels[ptr[y * displayLayout.Width + x]]];
					byte b = ptr[y * displayLayout.Width + x];
					try
					{
						dmd += _dmdMap[_dmdLevels[b]];
					}
					catch
					{
						dmd += _dmdMap[0];
					}


						
				}

				Console.SetCursorPosition(0, y);
				Console.Write(dmd);
				//Debug.WriteLine(dmd);
			}
		}

		public static Bitmap RawBytesToBitmap(byte[] rawPixels, int width, int height)
		{
			// 1. Create a blank bitmap with the target size and format
			Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);

			// 2. Lock the bitmap's bits to get a pointer to its memory space
			Rectangle rect = new Rectangle(0, 0, width, height);
			BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

			try
			{
				// 3. Copy the raw byte array directly to the bitmap's memory address
				Marshal.Copy(rawPixels, 0, bmpData.Scan0, rawPixels.Length);
			}
			finally
			{
				// 4. Always unlock the bits to prevent memory leaks or system locks
				bmp.UnlockBits(bmpData);
			}

			return bmp;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			try
			{
				_pinMame.StartGame("btmn_103");

			}
			catch { }
		}

		private void button3_Click(object sender, EventArgs e)
		{
			try
			{
				_pinMame.StopGame();

			}
			catch { }
		}
	}
}
