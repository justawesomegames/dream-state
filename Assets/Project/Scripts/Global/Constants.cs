using System.Collections;
using System.Collections.Generic;

namespace DreamState {
	namespace Global {
		public static class Constants {
			/// <summary>
			/// Input related constants
			/// </summary>
			public static class Input {
				public const string HORIZONTAL_AXIS = "Horizontal";
				public const string JUMP = "Jump";
				public const string DASH = "Dash";
			}

			public static class Collision {
				public const string ONE_WAY_PLATFORM_TAG = "Through";
			}
		}
	}
}