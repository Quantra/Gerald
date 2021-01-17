using System;
using Blish_HUD;
using Blish_HUD.Controls;

namespace Gerald.Controls
{
	public class Cursor : Image
	{
		protected override CaptureType CapturesInput()
		{
			return CaptureType.None;
		}
	}
}

