using System;
using System.Linq;

namespace PhysicsHand.Demo.Gadgets.Keypads
{
    /// <summary>
    /// An implementation of a KeypadController that only allows numeric inputs between 0 and 9.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class NumericKeypadController : KeypadController
    {
        // Public override method(s)
        #region Overrideable Input Method(s)
        public override void TrimInput()
        {
            // Perform the base input trimming.
            base.TrimInput();

            // Ensure input is non-numeric.
            if (input != null)
                input = new string(input.Where(c => Char.IsDigit(c)).ToArray());
        }
        #endregion
    }
}
