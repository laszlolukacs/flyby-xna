// <copyright file="ActorBase.cs" company=".">
// See license.md for details.
// </copyright>

namespace XnaFlyby.Library.Entities.Actors
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using XnaFlyby.Library.Components;

    /// <summary>
    /// An abstract base class for updateable and renderable elements of the scene.
    /// </summary>
    public abstract class ActorBase : RenderableBase, IUpdateableAndRenderable
    {
        /// <summary>
        /// The instance of the game class.
        /// </summary>
        protected Game game;

#if DEBUG
        /// <summary>
        /// The basic effect for rendering the bounding boxes.
        /// </summary>
        private readonly BasicEffect boundingBoxEffect;

        /// <summary>
        /// The indices for the bounding boxes.
        /// </summary>
        private readonly short[] boundingBoxIndices = 
        {
            0, 1, 1, 2, 2, 3, 3, 0, // Front edges
            4, 5, 5, 6, 6, 7, 7, 4, // Back edges
            0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
        };
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorBase"/> class.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="game">The game.</param>
        public ActorBase(GraphicsDevice graphics, Game game)
            : base(graphics)
        {
            this.game = game;
#if DEBUG
            this.IsBoundingBoxDrawingRequired = true;
            this.boundingBoxEffect = new BasicEffect(this.Device);
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorBase"/> class.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="game">The game.</param>
        /// <param name="model">The model.</param>
        public ActorBase(GraphicsDevice graphics, Game game, Model model)
            : this(graphics, game)
        {
            this.Model = model;
            this.InitializeMeshTags();
            this.InitializeBoundingBoxes();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ActorBase"/> is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this actor is alive.
        /// </summary>
        public bool IsAlive { get; set; }

#if DEBUG
        /// <summary>
        /// Gets or sets a value indicating whether drawing the bounding boxes are required.
        /// </summary>
        public bool IsBoundingBoxDrawingRequired { get; set; }
#endif

        /// <summary>
        /// Gets or sets the collection of the bounding boxes covering the specified model.
        /// </summary>
        /// <remarks>
        /// The bounding boxes can be used for collision detection.
        /// </remarks>
        public IList<BoundingBox> BoundingBoxes { get; set; }

        /// <summary>
        /// Gets or sets the model of the actor, which will be rendered.
        /// </summary>
        public Model Model { get; set; }

        /// <summary>
        /// Gets the gravity vector.
        /// </summary>
        protected static Vector3 Gravity
        {
            get
            {
                return new Vector3(0.0f, 0.0f, -10.0f);
            }
        }

        /// <summary>
        /// Initializes this actor instance.
        /// </summary>
        public override void Initialize()
        {
            this.IsAlive = true;
            base.Initialize();
        }

        /// <summary>
        /// Generates the tags for the currently used model's meshes.
        /// </summary>
        public void InitializeMeshTags()
        {
            foreach (var mesh in this.Model.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    var effect = meshPart.Effect as BasicEffect;
                    if (effect != null)
                    {
                        var basicEffect = effect;
                        var tag = new MeshTag(basicEffect.DiffuseColor, basicEffect.Texture, basicEffect.SpecularPower);
                        meshPart.Tag = tag;
                    }
                }
            }
        }

        /// <summary>
        /// Creates the bounding boxes for each mesh of the currently used model.
        /// </summary>
        public void InitializeBoundingBoxes()
        {
            this.BoundingBoxes = new List<BoundingBox>();

            // gets the transforms of the model's parts
            Matrix[] transforms = new Matrix[this.Model.Bones.Count];
            this.Model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (var mesh in this.Model.Meshes)
            {
                Matrix meshTransform = transforms[mesh.ParentBone.Index];

                // declares the initial min and max xyz values for the mesh
                var meshMax = new Vector3(float.MinValue);
                var meshMin = new Vector3(float.MaxValue);

                foreach (var meshPart in mesh.MeshParts)
                {
                    // gets the current stride size of the vertex buffer in bytes
                    // this is the difference between two vertices which are located in the vertex buffer
                    int strideSize = meshPart.VertexBuffer.VertexDeclaration.VertexStride;

                    var vertexData = new VertexPositionNormalTexture[meshPart.NumVertices];
                    meshPart.VertexBuffer.GetData(meshPart.VertexOffset * strideSize, vertexData, 0, meshPart.NumVertices, strideSize);

                    // finds minimum and maximum xyz values for this mesh part
                    var vertPosition = new Vector3();

                    for (int i = 0; i < vertexData.Length; i++)
                    {
                        vertPosition = vertexData[i].Position;

                        // updates min and max values from this vertex
                        meshMin = Vector3.Min(meshMin, vertPosition);
                        meshMax = Vector3.Max(meshMax, vertPosition);
                    }
                }

                // transforms by mesh bone matrix
                meshMin = Vector3.Transform(meshMin, meshTransform);
                meshMax = Vector3.Transform(meshMax, meshTransform);

                // adds the bounding box to the collection
                this.BoundingBoxes.Add(new BoundingBox(meshMin, meshMax));
            }
        }

        /// <summary>
        /// Performs the behavior of this element.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public virtual void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Performs the rendering of this element.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="deltaT">The delta T.</param>
        public override void Draw(Camera camera, float deltaT)
        {
#if DEBUG
            if (this.IsBoundingBoxDrawingRequired)
            {
                // draws the bounding boxes
                foreach (var boundingBox in this.BoundingBoxes)
                {
                    Vector3[] corners = boundingBox.GetCorners();
                    VertexPositionColor[] boundingBoxVertices = new VertexPositionColor[corners.Length];

                    for (int i = 0; i < corners.Length; i++)
                    {
                        boundingBoxVertices[i] = new VertexPositionColor(corners[i], Color.White);
                    }

                    this.boundingBoxEffect.World = this.World;
                    this.boundingBoxEffect.View = camera.View;
                    this.boundingBoxEffect.Projection = camera.Projection;

                    this.boundingBoxEffect.CurrentTechnique.Passes[0].Apply();

                    this.Device.DrawUserIndexedPrimitives(PrimitiveType.LineList, boundingBoxVertices, 0, 8, this.boundingBoxIndices, 0, 12);
                }
            }
#endif

            base.Draw(camera, deltaT);
        }

        /// <summary>
        /// Performs the rendering of this element.
        /// </summary>
        /// <param name="view">The view matrix.</param>
        /// <param name="projection">The projection matrix.</param>
        /// <param name="deltaT">The delta T.</param>
        public override void Draw(Matrix view, Matrix projection, float deltaT)
        {
#if DEBUG
            if (this.IsBoundingBoxDrawingRequired)
            {
                foreach (var boundingBox in this.BoundingBoxes)
                {
                    Vector3[] corners = boundingBox.GetCorners();
                    VertexPositionColor[] boundingBoxVertices = new VertexPositionColor[corners.Length];

                    for (int i = 0; i < corners.Length; i++)
                    {
                        boundingBoxVertices[i] = new VertexPositionColor(corners[i], Color.White);
                    }

                    this.boundingBoxEffect.World = this.World;
                    this.boundingBoxEffect.View = view;
                    this.boundingBoxEffect.Projection = projection;

                    this.boundingBoxEffect.CurrentTechnique.Passes[0].Apply();

                    this.Device.DrawUserIndexedPrimitives(PrimitiveType.LineList, boundingBoxVertices, 0, 8, this.boundingBoxIndices, 0, 12);
                }
            }
#endif

            base.Draw(view, projection, deltaT);
        }
    }
}
