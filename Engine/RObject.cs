using MathematicalEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicRender.Engine {

    public class RObject {

        public int id;
        public string name;

        public int num_vertices;
        public int num_frames;
        public int curr_frame;

        public float avgRadius;
        public float maxRadius;

        public RTexture texture;

        public Vec3f pos;
        public Vec3f dir;
        public Vec3f ux;
        public Vec3f uy;
        public Vec3f uz;

        public Mat4f model;
        public Mat4f world;
        public Mat4f view;
        public Mat4f proj;

        public List<RPolygon> polygons = new List<RPolygon>();
        public List<RPolygon> polygons_model = new List<RPolygon>();
        public List<RPolygon> polygons_world = new List<RPolygon>();
        public List<RPolygon> polygons_view = new List<RPolygon>();
        public List<RPolygon> polygons_proj = new List<RPolygon>();
        public List<RPolygon> polygons_raster = new List<RPolygon>();

        public bool visible;

        public void calculateModel() {

            polygons_model = new List<RPolygon>();
            foreach (RPolygon poly in polygons) {

                RVertex vertex1 = new RVertex(poly.vertex[0]);
                vertex1.position = new Vec3f(model * poly.v0);

                RVertex vertex2 = new RVertex(poly.vertex[1]);
                vertex2.position = new Vec3f(model * poly.v1);

                RVertex vertex3 = new RVertex(poly.vertex[2]);
                vertex3.position = new Vec3f(model * poly.v2);

                polygons_model.Add(new RPolygon(vertex1, vertex2, vertex3, poly.rasterType, poly.texture));
            }
        }

        public void calculateWorld() {

            polygons_world = new List<RPolygon>();
            foreach (RPolygon poly in polygons_model) {

                RVertex vertex1 = new RVertex(poly.vertex[0]);
                vertex1.position = new Vec3f(world * poly.v0);

                RVertex vertex2 = new RVertex(poly.vertex[1]);
                vertex2.position = new Vec3f(world * poly.v1);

                RVertex vertex3 = new RVertex(poly.vertex[2]);
                vertex3.position = new Vec3f(world * poly.v2);

                polygons_world.Add(new RPolygon(vertex1, vertex2, vertex3, poly.rasterType, poly.texture));
            }
        }

        public void calculateView(bool needToSort) {

            polygons_view = new List<RPolygon>();
            foreach (RPolygon poly in polygons_world) {

                RVertex vertex1 = new RVertex(poly.vertex[0]);
                vertex1.position = new Vec3f(view * poly.v0);

                RVertex vertex2 = new RVertex(poly.vertex[1]);
                vertex2.position = new Vec3f(view * poly.v1);

                RVertex vertex3 = new RVertex(poly.vertex[2]);
                vertex3.position = new Vec3f(view * poly.v2);

                RPolygon newPolygon = new RPolygon(vertex1, vertex2, vertex3, poly.rasterType, poly.texture);

                polygons_view.Add(newPolygon);
            }

            if (needToSort) {
                polygons_view = polygons_view.OrderBy(p => p.center).ToList();
            }
        }

        public void calculateProj() {

            polygons_proj = new List<RPolygon>();
            foreach (RPolygon poly in polygons_view) {


                RVertex vertex1 = new RVertex(poly.vertex[0]);
                vertex1.position = new Vec3f(proj * poly.v0);

                RVertex vertex2 = new RVertex(poly.vertex[1]);
                vertex2.position = new Vec3f(proj * poly.v1);

                RVertex vertex3 = new RVertex(poly.vertex[2]);
                vertex3.position = new Vec3f(proj * poly.v2);

                RPolygon newPolygon = new RPolygon(vertex1, vertex2, vertex3, poly.rasterType, poly.texture);
                float cosTheta = newPolygon.normalCalc().dot(GlobalDirection.forward3f);
                newPolygon.visible = cosTheta >= 0.0f ? false : true;

                if (newPolygon.visible) {
                    polygons_proj.Add(newPolygon);
                }
            }
        }

        public void calculateRaster(float width, float height, bool isNDC) {

            polygons_raster = new List<RPolygon>();
            foreach (RPolygon poly in polygons_proj) {

                RVertex vertex1 = new RVertex(poly.vertex[0]);
                vertex1.position.x = isNDC ? (poly.v0.x + 1.0f) / 2.0f * width : poly.v0.x * width;
                vertex1.position.y = isNDC ? (poly.v0.y + 1.0f) / 2.0f * height : poly.v0.y * height;

                RVertex vertex2 = new RVertex(poly.vertex[1]);
                vertex2.position.x = isNDC ? (poly.v1.x + 1.0f) / 2.0f * width : poly.v1.x * width;
                vertex2.position.y = isNDC ? (poly.v1.y + 1.0f) / 2.0f * height : poly.v1.y * height;

                RVertex vertex3 = new RVertex(poly.vertex[2]);
                vertex3.position.x = isNDC ? (poly.v2.x + 1.0f) / 2.0f * width : poly.v2.x * width;
                vertex3.position.y = isNDC ? (poly.v2.y + 1.0f) / 2.0f * height : poly.v2.y * height;

                polygons_raster.Add(new RPolygon(vertex1, vertex2, vertex3, poly.rasterType, poly.texture));
            }
        }

    }
}
