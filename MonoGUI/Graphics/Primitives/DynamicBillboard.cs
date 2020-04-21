using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


/*
 * File:		DynamicBillboard
 * Purpose:		Class for rendering unlimited billboards 
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-06-14  RW	
 * 
 * History:		2010-06-14  RW  Created
 * 
 */
namespace MonoGUI.Graphics
{

    /// <summary>
    /// Class for rendering unlimited billboards
    /// </summary>
    public class DynamicBillboard
    {

        #region Private membets

        private List<Billboard> fBillboards = new List<Billboard>();

        private GraphicsDevice fDevice;
        private bool fUseVertexBuffer;
        private BillboardMode fMode = BillboardMode.PointList;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current number of objects
        /// </summary>
        public int NumberOfObjects
        {
            get
            {
                int count = 0;
                foreach(Billboard billboard in fBillboards)
                    count+=billboard.NumberOfObjects;
                return count;
            }
        }

        /// <summary>
        /// Get/set if should be using the graphic card to store the primitives
        /// </summary>
        public bool UseVertexBuffer
        {
            get
            {
                return fUseVertexBuffer;
            }
            set
            {
                fUseVertexBuffer = value;
                foreach(Billboard billboard in fBillboards)
                    billboard.UseVertexBuffer = value;
            }
        }

        /// <summary>
        /// Get/set the type of mode of the billboards
        /// </summary>
        public BillboardMode Mode
        {
            get
            {
                return fMode;
            }
            set
            {
                fMode = value;
                foreach (Billboard billboard in fBillboards)
                    billboard.Mode = value;
            }
        }

        #endregion

        #region Constructor / destructor

        /// <summary>
        /// Creates a new dynamic billboard object.
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public DynamicBillboard(GraphicsDevice graphicsDevice)
        {
            fDevice = graphicsDevice;
            Clear();
        }

        /// <summary>
        /// Called when the dynamic billboard object is destroyed.
        /// </summary>
        ~DynamicBillboard()
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Add a new object
        /// </summary>
        public bool AddObject(Vector3 position, Color color, float size)
        {
            Billboard billboard;
            if (fBillboards.Count == 0)
            {
                billboard = new Billboard(fDevice);
                billboard.UseVertexBuffer = fUseVertexBuffer;
                billboard.Mode = fMode;
                fBillboards.Add(billboard);
            }
            else
            {
                billboard = fBillboards[fBillboards.Count - 1];
                if (billboard.NumberOfObjects >= Billboard.MaxObjects)
                {
                    billboard = new Billboard(fDevice);
                    billboard.UseVertexBuffer = fUseVertexBuffer;
                    billboard.Mode = fMode;
                    fBillboards.Add(billboard);
                }
            }
            return billboard.AddObject(position, color, size);
        }

        /// <summary>
        /// Clears all vectors from the dynamic billboard object.
        /// </summary>
        public void Clear()
        {
            fBillboards.Clear();
        }

        #endregion

        #region Draw method

        /// <summary>
        /// Renders the dynamic billboard object.
        /// </summary>
        public void Render()
        {
            foreach(Billboard billboard in fBillboards)
                billboard.Render();
        }

        #endregion
         
    }


}
