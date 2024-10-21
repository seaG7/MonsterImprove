
using UnityEngine;
using UnityEditor;
#if UNITY_2019_1_OR_NEWER
using UnityEditor.ShortcutManagement;
#endif
using Technie.PhysicsCreator.Rigid;
using Technie.PhysicsCreator.Skinned;

namespace Technie.PhysicsCreator
{
#if UNITY_2019_1_OR_NEWER
	// NB: Lots of shortchuts have to be registered as a global shortcut (no context type specified) because if we register
	// a context it only fires when the window has direct focus, which often isn't the case as the user is painting
	// (technically the scene view has focus, even though the hull painter window is the selected window)
	public class Shortcuts
	{
		private static bool IsRigidCreatorActive(out RigidColliderCreatorWindow rigidWindow)
		{
			if (RigidColliderCreatorWindow.IsOpen() && RigidColliderCreatorWindow.instance != null && RigidColliderCreatorWindow.instance.ShouldReceiveShortcuts())
			{
				rigidWindow = RigidColliderCreatorWindow.instance;
				return true;
			}
			rigidWindow = null;
			return false;
		}

		private static bool IsSkinnedCreatorActive(out SkinnedColliderCreatorWindow skinnedWindow)
		{
			if (SkinnedColliderCreatorWindow.IsOpen() && SkinnedColliderCreatorWindow.instance != null && SkinnedColliderCreatorWindow.instance.ShouldReceiveShortcuts())
			{
				skinnedWindow = SkinnedColliderCreatorWindow.instance;
				return true;
			}
			skinnedWindow = null;
			return false;
		}

		private static bool IsCreatorActive(out ICreatorWindow window)
		{
			if (RigidColliderCreatorWindow.IsOpen() && RigidColliderCreatorWindow.instance != null && RigidColliderCreatorWindow.instance.ShouldReceiveShortcuts())
			{
				window = RigidColliderCreatorWindow.instance;
				return true;
			}
			if (SkinnedColliderCreatorWindow.IsOpen() && SkinnedColliderCreatorWindow.instance != null && SkinnedColliderCreatorWindow.instance.ShouldReceiveShortcuts())
			{
				window = SkinnedColliderCreatorWindow.instance;
				return true;
			}
			window = null;
			return false;
		}

		[Shortcut("Collider Creator/Add Hull", KeyCode.Q, ShortcutModifiers.Shift)]
		public static void AddHull()
		{
			/*
			if (IsCreatorActive(out ICreatorWindow window))
			{
				window.AddHull();
				window.Repaint();
			}
			*/
			
			if (IsRigidCreatorActive(out RigidColliderCreatorWindow rigidWindow))
			{
				rigidWindow.AddHull();
				rigidWindow.Repaint();
			}
			else if (IsSkinnedCreatorActive(out SkinnedColliderCreatorWindow skinnedWindow))
			{
				// TOOD!
			}
		}

		[Shortcut("Collider Creator/Generate colliders", KeyCode.W, ShortcutModifiers.Shift)]
		public static void Generate()
		{
			/*
			if (RigidColliderCreatorWindow.IsOpen())
			{
				RigidColliderCreatorWindow.instance.GenerateColliders();
			}
			*/
			if (IsCreatorActive(out ICreatorWindow window))
			{
				window.GenerateColliders();
			}
		}

		[Shortcut("Collider Creator/Delete generated colliders", KeyCode.E, ShortcutModifiers.Shift)]
		public static void DeleteGenerated()
		{
			/*
			if (RigidColliderCreatorWindow.IsOpen())
			{
				RigidColliderCreatorWindow.instance.DeleteGenerated();
				RigidColliderCreatorWindow.instance.Repaint();
			}
			*/
			if (IsCreatorActive(out ICreatorWindow window))
			{
				window.DeleteGenerated();
				window.Repaint();
			}
		}

		[Shortcut("Collider Creator/Stop painting", KeyCode.S, ShortcutModifiers.Shift)]
		public static void StopPainting()
		{
			/*
			if (RigidColliderCreatorWindow.IsOpen())
			{
				RigidColliderCreatorWindow.instance.StopPainting();
				RigidColliderCreatorWindow.instance.Repaint();
			}
			*/
			if (IsCreatorActive(out ICreatorWindow window))
			{
				window.StopPainting();
				window.Repaint();
			}
		}

		[Shortcut("Collider Creator/Paint all faces", KeyCode.A, ShortcutModifiers.Shift)]
		public static void PaintAll()
		{
			/*
			if (RigidColliderCreatorWindow.IsOpen())
			{
				RigidColliderCreatorWindow.instance.PaintAllFaces();
			}
			*/
			if (IsCreatorActive(out ICreatorWindow window))
			{
				window.PaintAllFaces();
			}
		}

		[Shortcut("Collider Creator/Unpaint all faces", KeyCode.Z, ShortcutModifiers.Shift)]
		public static void UnpaintAllFaces()
		{
			/*
			if (RigidColliderCreatorWindow.IsOpen())
			{
				RigidColliderCreatorWindow.instance.UnpaintAllFaces();
			}
			*/
			if (IsCreatorActive(out ICreatorWindow window))
			{
				window.UnpaintAllFaces();
			}
		}

		[Shortcut("Collider Creator/Cycle brush size", KeyCode.Alpha1, ShortcutModifiers.Shift)]
		public static void AdvanceBrushSize()
		{
			/*
			if (RigidColliderCreatorWindow.IsOpen())
			{
				RigidColliderCreatorWindow.instance.AdvanceBrushSize();
				RigidColliderCreatorWindow.instance.Repaint();
			}
			*/
			if (IsCreatorActive(out ICreatorWindow window))
			{
				window.AdvanceBrushSize();
				window.Repaint();
			}
		}

		[Shortcut("Collider Creator/Select Pipette", KeyCode.BackQuote, ShortcutModifiers.Shift)]
		public static void SelectPipette()
		{
			/*
			if (RigidColliderCreatorWindow.IsOpen())
			{
				RigidColliderCreatorWindow.instance.SelectPipette();
				RigidColliderCreatorWindow.instance.Repaint();
			}
			*/
			if (IsCreatorActive(out ICreatorWindow window))
			{
				window.SelectPipette();
				window.Repaint();
			}
		}

		[Shortcut("Collider Creator/Cycle hull type", KeyCode.T, ShortcutModifiers.Shift)]
		public static void CycleHullType()
		{
			if (RigidColliderCreatorWindow.IsOpen())
			{
				RigidColliderCreatorWindow.instance.CycleHullType();
				RigidColliderCreatorWindow.instance.Repaint();
			}
			/*
			if (IsCreatorActive(out ICreatorWindow window))
			{
				window.CycleHullType();
				window.Repaint();
			}
			*/
		}

		[Shortcut("Collider Creator/Select next hull", KeyCode.Equals, ShortcutModifiers.Shift)]
		public static void SelectNextHull()
		{
			if (RigidColliderCreatorWindow.IsOpen())
			{
				RigidColliderCreatorWindow.instance.AdvanceSelectedHull(1);
				RigidColliderCreatorWindow.instance.Repaint();
			}
		}

		[Shortcut("Collider Creator/Select previous hull", KeyCode.Minus, ShortcutModifiers.Shift)]
		public static void SelectPrevHull()
		{
			if (RigidColliderCreatorWindow.IsOpen())
			{
				RigidColliderCreatorWindow.instance.AdvanceSelectedHull(-1);
				RigidColliderCreatorWindow.instance.Repaint();
			}
		}

		[Shortcut("Collider Creator/Toggle as child", KeyCode.Y, ShortcutModifiers.Shift)]
		public static void ToggleIsChild()
		{
			if (RigidColliderCreatorWindow.IsOpen())
			{
				RigidColliderCreatorWindow.instance.ToggleIsChild();
				RigidColliderCreatorWindow.instance.Repaint();
			}
		}

		[Shortcut("Collider Creator/Toggle is trigger", KeyCode.U, ShortcutModifiers.Shift)]
		public static void ToggleIsTrigger()
		{
			if (RigidColliderCreatorWindow.IsOpen())
			{
				RigidColliderCreatorWindow.instance.ToggleIsTrigger();
				RigidColliderCreatorWindow.instance.Repaint();
			}
		}

		[Shortcut("Collider Creator/Delete active hull", KeyCode.Backspace, ShortcutModifiers.Shift)]
		public static void DeleteActiveHull()
		{
			if (RigidColliderCreatorWindow.IsOpen())
			{
				RigidColliderCreatorWindow.instance.DeleteActiveHull();
				RigidColliderCreatorWindow.instance.Repaint();
			}
		}

	} // ssalc Shortcuts
#endif // UNITY_2019_1_OR_NEWER
} // ecapseman Technie.PhysicsCreator
