using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Blish_HUD.GameService;
using Gerald.Controls;

namespace Gerald
{
	[Export(typeof(Blish_HUD.Modules.Module))]
	public class Gerald : Blish_HUD.Modules.Module
	{

		private static readonly Logger Logger = Logger.GetLogger<Module>();

		#region Service Managers
		internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
		internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
		internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
		internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
		#endregion

		[ImportingConstructor]
		public Gerald([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

		private SettingEntry<int> _settingCursorThickness;

		protected override void DefineSettings(SettingCollection settings)
		{
			_settingCursorThickness = settings.DefineSetting("CursorThickness", 3, "Cursor Thickness");
            //_settingCursorThickness.SetRange(1, 20);

			_settingCursorThickness.SettingChanged += UpdateCursorThickness;

        }

		private Texture2D dummyTexture;
		private Image lineH;
		private Image lineV;
		private bool updateCursor = true;
		private int skipUpdate = 0;
		private Point lastMouseLoc;
		private int useLastLoc = 0;

		protected override void Initialize()
		{
			dummyTexture = new Texture2D(Graphics.GraphicsDevice, 1, 1);
			dummyTexture.SetData(new Color[] { Color.Magenta });

			lineH = new Cursor
			{
				Parent = Graphics.SpriteScreen,
				Size = new Point(1920, _settingCursorThickness.Value),
				Location = new Point(0, 0),
				Texture = dummyTexture,
				Opacity = 1.0f
			};

			lineV = new Cursor
			{
				Parent = Graphics.SpriteScreen,
				Size = new Point(_settingCursorThickness.Value, 1080),
				Location = new Point(0, 0),
				Texture = dummyTexture,
				Opacity = 1.0f
			};

			//Input.Mouse.MouseMoved += UpdateCursorPosition;

			// Preventing updates whilst left button is pressed works well
			Input.Mouse.LeftMouseButtonPressed += DisableCursorUpdates;
			Input.Mouse.LeftMouseButtonReleased += EnableCursorUpdates;
			// Preventing updates whilst right button is pressed doesnt stop the cursor jumping when it is released
			//Input.Mouse.RightMouseButtonPressed += DisableCursorUpdates;
			//Input.Mouse.RightMouseButtonReleased += EnableCursorUpdates;

			// Perhaps a solution to stop the cursor jumping is to store the mouseLoc on RightMouseButtonPressed and use this
			// instead of Mouse.Position immediately after RightMouseButtonReleased?  Nope.

			// Try logging mouse pos and see what is going on.  Perhaps using update method instead of mousemoved helps this?
			// Ask on discord too.

			lastMouseLoc = Input.Mouse.Position;
		}

		protected override async Task LoadAsync()
		{

		}

		protected override void OnModuleLoaded(EventArgs e)
		{

			// Base handler must be called
			base.OnModuleLoaded(e);
		}

		protected override void Update(GameTime gameTime)
		{
			UpdateCursorPosition();
        }

		/// <inheritdoc />
		protected override void Unload()
		{
			// Unload here

			// All static members must be manually unset
		}

		//private void UpdateCursorPosition(object sender, MouseEventArgs e)
		private void UpdateCursorPosition()
		{
			if (!updateCursor) { return; }

			//Point mouseLoc = Input.Mouse.Position;
			Point mouseLoc = Blish_HUD.InputService.Input.Mouse.Position;
			
			//double moveDistance = PointDistance(lastMouseLoc, mouseLoc);
			//lastMouseLoc = mouseLoc;

			// I am not sure if this actually helps stop the cursor jumping around when the right mouse button is released
			//if (moveDistance > 10d) { return; }

			//if (useLastLoc > 0)
   //         {
			//	mouseLoc = lastMouseLoc;
			//	useLastLoc -= 1;
   //         }

			lineH.Location = new Point(lineH.Location.X, mouseLoc.Y - _settingCursorThickness.Value / 2);
			lineV.Location = new Point(mouseLoc.X - _settingCursorThickness.Value / 2, lineV.Location.Y);
		}

		private void UpdateCursorThickness(object sender = null, ValueChangedEventArgs<int> e = null)
		{
			lineH.Size = new Point(lineH.Size.X, _settingCursorThickness.Value);
			lineV.Size = new Point(_settingCursorThickness.Value, lineV.Size.Y);
		}

		private void DisableCursorUpdates(object sender, MouseEventArgs e)
        {
			updateCursor = false;
			lastMouseLoc = Input.Mouse.Position;
			useLastLoc = 50;
		}

		private void EnableCursorUpdates(object sender, MouseEventArgs e)
        {
			updateCursor = true;
        }

		private double PointDistance(Point pointA, Point pointB)
        {
			Point dif = pointA - pointB;
			return Math.Sqrt(dif.X^2 + dif.Y^2);
        }

	}

}
