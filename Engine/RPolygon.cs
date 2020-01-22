using MathematicalEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicRender.Engine {

    public class RPolygon {

        public int id;
        public bool visible = true;
        public bool sorted = false;
        public int[] order = new int[3] { 0, 1, 2 };
        public RasterType rasterType;
        public RTexture texture;
        public RVertex[] vertex = new RVertex[3];

        public Vec3f v0 { get => vertex[0].position; set => vertex[0].position = value; }
        public Vec3f v1 { get => vertex[1].position; set => vertex[1].position = value; }
        public Vec3f v2 { get => vertex[2].position; set => vertex[2].position = value; }

        public Vec4f c0 { get => vertex[0].color; set => vertex[0].color = value; }
        public Vec4f c1 { get => vertex[1].color; set => vertex[1].color = value; }
        public Vec4f c2 { get => vertex[2].color; set => vertex[2].color = value; }

        public Vec2f t0 { get => vertex[0].textureCoordinates; set => vertex[0].textureCoordinates = value; }
        public Vec2f t1 { get => vertex[1].textureCoordinates; set => vertex[1].textureCoordinates = value; }
        public Vec2f t2 { get => vertex[2].textureCoordinates; set => vertex[2].textureCoordinates = value; }

        public Vec3f n0 { get => vertex[0].normal; set => vertex[0].normal = value; }
        public Vec3f n1 { get => vertex[1].normal; set => vertex[1].normal = value; }
        public Vec3f n2 { get => vertex[2].normal; set => vertex[2].normal = value; }

        public float x0 { get => v0.x; set => v0.x = value; }
        public float y0 { get => v0.y; set => v0.y = value; }
        public float z0 { get => v0.z; set => v0.z = value; }

        public float x1 { get => v1.x; set => v1.x = value; }
        public float y1 { get => v1.y; set => v1.y = value; }
        public float z1 { get => v1.z; set => v1.z = value; }

        public float x2 { get => v2.x; set => v2.x = value; }
        public float y2 { get => v2.y; set => v2.y = value; }
        public float z2 { get => v2.z; set => v2.z = value; }

        public float r0 { get => vertex[0].r; set => vertex[0].r = value; }
        public float g0 { get => vertex[0].g; set => vertex[0].g = value; }
        public float b0 { get => vertex[0].b; set => vertex[0].b = value; }

        public float r1 { get => vertex[1].r; set => vertex[1].r = value; }
        public float g1 { get => vertex[1].g; set => vertex[1].g = value; }
        public float b1 { get => vertex[1].b; set => vertex[1].b = value; }

        public float r2 { get => vertex[2].r; set => vertex[2].r = value; }
        public float g2 { get => vertex[2].g; set => vertex[2].g = value; }
        public float b2 { get => vertex[2].b; set => vertex[2].b = value; }

        public float tu0 { get => vertex[0].u; set => vertex[0].u = value; }
        public float tv0 { get => vertex[0].v; set => vertex[0].v = value; }

        public float tu1 { get => vertex[1].u; set => vertex[1].u = value; }
        public float tv1 { get => vertex[1].v; set => vertex[1].v = value; }

        public float tu2 { get => vertex[2].u; set => vertex[2].u = value; }
        public float tv2 { get => vertex[2].v; set => vertex[2].v = value; }

        public float nx0 { get => vertex[0].nx; set => vertex[0].nx = value; }
        public float ny0 { get => vertex[0].ny; set => vertex[0].ny = value; }
        public float nz0 { get => vertex[0].nz; set => vertex[0].nz = value; }

        public float nx1 { get => vertex[1].nx; set => vertex[1].nx = value; }
        public float ny1 { get => vertex[1].ny; set => vertex[1].ny = value; }
        public float nz1 { get => vertex[1].nz; set => vertex[1].nz = value; }

        public float nx2 { get => vertex[2].nx; set => vertex[2].nx = value; }
        public float ny2 { get => vertex[2].ny; set => vertex[2].ny = value; }
        public float nz2 { get => vertex[2].nz; set => vertex[2].nz = value; }

        public Vec3f normal;
        public Vec3f center;

        public enum RasterType : int {
            FlatColor,
            InterpolatedColor,
            Textured
        }

        public RPolygon() {
            this.id = ++lastId;
        }

        public RPolygon(RPolygon other) {

            this.vertex = new RVertex[3];
            this.vertex[0] = new RVertex(other.vertex[0]);
            this.vertex[1] = new RVertex(other.vertex[1]);
            this.vertex[2] = new RVertex(other.vertex[2]);
            this.rasterType = other.rasterType;
            this.texture = other.texture;
            this.normal = other.normal;
            this.center = other.center;
            this.id = ++lastId;
        }

        public RPolygon(RVertex vertex1, RVertex vertex2, RVertex vertex3) {

            this.vertex[0] = vertex1;
            this.vertex[1] = vertex2;
            this.vertex[2] = vertex3;

            this.center = centerCalc();
            this.id = ++lastId;
        }

        public RPolygon(RVertex vertex1, RVertex vertex2, RVertex vertex3, bool isNormalCalc) {

            this.vertex[0] = vertex1;
            this.vertex[1] = vertex2;
            this.vertex[2] = vertex3;

            if (isNormalCalc) {
                this.normal = normalizedNormalCalc();
                this.vertex[0].normal = this.normal;
                this.vertex[1].normal = this.normal;
                this.vertex[2].normal = this.normal;
            }

            this.center = centerCalc();
            this.id = ++lastId;
        }

        public RPolygon(RVertex vertex1, RVertex vertex2, RVertex vertex3, RasterType rasterType) {

            this.vertex[0] = vertex1;
            this.vertex[1] = vertex2;
            this.vertex[2] = vertex3;

            this.rasterType = rasterType;

            this.center = centerCalc();
            this.id = ++lastId;
        }

        public RPolygon(RVertex vertex1, RVertex vertex2, RVertex vertex3, RasterType rasterType, RTexture texture) {

            this.vertex[0] = vertex1;
            this.vertex[1] = vertex2;
            this.vertex[2] = vertex3;

            this.rasterType = rasterType;
            this.texture = texture;

            this.center = centerCalc();
            this.id = ++lastId;
        }

        public RPolygon(RVertex vertex1, RVertex vertex2, RVertex vertex3, RasterType rasterType, bool isNormalCalc) {

            this.vertex[0] = vertex1;
            this.vertex[1] = vertex2;
            this.vertex[2] = vertex3;

            this.rasterType = rasterType;

            if (isNormalCalc) {
                this.normal = normalizedNormalCalc();
                this.vertex[0].normal = this.normal;
                this.vertex[1].normal = this.normal;
                this.vertex[2].normal = this.normal;
            }

            this.center = centerCalc();
            this.id = ++lastId;
        }

        public RPolygon reSort() {
            RPolygon result = new RPolygon(this);
            result.sort();
            return result;
        }

        public void sort() {

            RVertex A = vertex[0];
            RVertex B = vertex[1];
            RVertex C = vertex[2];
            RVertex Temp = new RVertex();
            int t;
            bool isSwitchAction = false;

            if (B.y < A.y) {

                Temp = A;
                A = B;
                B = Temp;

                t = order[0];
                order[0] = order[1];
                order[1] = t;

                isSwitchAction = true;
            }

            if (C.y < A.y) {

                Temp = A;
                A = C;
                C = Temp;

                t = order[0];
                order[0] = order[2];
                order[2] = t;

                isSwitchAction = true;
            }

            if (A.y > C.y) {

                Temp = C;
                C = A;
                A = Temp;

                t = order[2];
                order[2] = order[0];
                order[0] = t;

                isSwitchAction = true;
            }

            if (B.y > C.y) {

                Temp = C;
                C = B;
                B = Temp;

                t = order[2];
                order[2] = order[1];
                order[1] = t;

                isSwitchAction = true;
            }

            if (isSwitchAction) {
                vertex[0] = A;
                vertex[1] = B;
                vertex[2] = C;
            }
            sorted = true;
        }

        public RPolygon reClock() {

            return new RPolygon(vertex[0], vertex[2], vertex[1]);
        }

        public bool isEpsilonGeometry() {
            if ((GeneralVariables.FCMP(v0.x, v1.x) && GeneralVariables.FCMP(v1.x, v2.x)) || (GeneralVariables.FCMP(v0.y, v1.y) && GeneralVariables.FCMP(v1.y, v2.y)))
                return true;
            else
                return false;
        }

        public bool isOffScreen(Vec2f leftTop, Vec2f rightBottom) {
            if (sorted) {
                if (v2.y < leftTop.y || v0.y > rightBottom.y || (v0.x < leftTop.x && v1.x < leftTop.x && v2.x < leftTop.x) || (v0.x > rightBottom.x && v1.x > rightBottom.x && v2.x > rightBottom.x))
                    return true;
                else
                    return false;
            }
            else {
                RPolygon temp = reSort();
                if (temp.v2.y < leftTop.y || temp.v0.y > rightBottom.y || (temp.v0.x < leftTop.x && temp.v1.x < leftTop.x && temp.v2.x < leftTop.x) || (temp.v0.x > rightBottom.x && temp.v1.x > rightBottom.x && temp.v2.x > rightBottom.x))
                    return true;
                else
                    return false;
            }
        }

        public bool isFlatTop() {
            if (GeneralVariables.FCMP(v0.y, v1.y))
                return true;
            else {
                return false;
            }
        }

        public bool isFlatBottom() {
            if (GeneralVariables.FCMP(v1.y, v2.y))
                return true;
            else {
                return false;
            }
        }

        public Vec2f flatCoeff() {
            if (sorted) {
                float new_x = v0.x + (v1.y - v0.y) * (v2.x - v0.x) / (v2.y - v0.y);
                return new Vec2f(new_x, v1.y);
            }
            else {
                RPolygon temp = reSort();
                float new_x = temp.v0.x + (temp.v1.y - temp.v0.y) * (temp.v2.x - temp.v0.x) / (temp.v2.y - temp.v0.y);
                return new Vec2f(new_x, temp.v1.y);
            }
        }

        public bool clockWise() {

            if (sorted) {
                return edgeFunction(v0, v1, v2) < 0.0f ? true : false;
            }
            else {
                RPolygon temp = reSort();
                return edgeFunction(temp.v0, temp.v1, temp.v2) < 0.0f ? true : false;
            }
        }

        public RPolygon getBottomFlat() {
            if (sorted) {
                return getBottomFlat(this);
            }
            else {
                return getBottomFlat(reSort());
            }
        }

        public static RPolygon getBottomFlat(RPolygon temp) {

            float v0z = 1.0f / temp.v0.z;
            float v1z = 1.0f / temp.v1.z;
            float v2z = 1.0f / temp.v2.z;

            Vec2f flatEdge = temp.flatCoeff();
            Vec3f w = temp.nlambdas(flatEdge);
            float z = 1.0f / (w.x * v0z + w.y * v1z + w.z * v2z);

            Trianglef geometry = new Trianglef(
                new Vec3f(temp.v0),
                new Vec3f(flatEdge, z),
                new Vec3f(temp.v1)
            );

            Trianglef color = new Trianglef(
                new Vec3f(temp.c0),
                new Vec3f(
                    (temp.r0 * v0z * w.x + temp.r1 * v1z * w.y + temp.r2 * v2z * w.z) * z,
                    (temp.g0 * v0z * w.x + temp.g1 * v1z * w.y + temp.g2 * v2z * w.z) * z,
                    (temp.b0 * v0z * w.x + temp.b1 * v1z * w.y + temp.b2 * v2z * w.z) * z
                ),
                new Vec3f(temp.c1)
            );

            Trianglef textureCoords = new Trianglef(
                new Vec3f(temp.t0),
                new Vec3f(
                    (temp.tu0 * v0z * w.x + temp.tu1 * v1z * w.y + temp.tu2 * v2z * w.z) * z,
                    (temp.tv0 * v0z * w.x + temp.tv1 * v1z * w.y + temp.tv2 * v2z * w.z) * z,
                    0.0f
                ),
                new Vec3f(temp.t1)
            );

            Trianglef normals = new Trianglef(
                new Vec3f(temp.n0),
                new Vec3f(
                    (temp.nx0 * v0z * w.x + temp.nx1 * v1z * w.y + temp.nx2 * v2z * w.z) * z,
                    (temp.ny0 * v0z * w.x + temp.ny1 * v1z * w.y + temp.ny2 * v2z * w.z) * z,
                    (temp.nz0 * v0z * w.x + temp.nz1 * v1z * w.y + temp.nz2 * v2z * w.z) * z
                ),
                new Vec3f(temp.n1)
            );

            return new RPolygon(
                new RVertex(geometry.v0, color.v0, normals.v0, new Vec2f(textureCoords.v0)),
                new RVertex(geometry.v1, color.v1, normals.v1, new Vec2f(textureCoords.v1)),
                new RVertex(geometry.v2, color.v2, normals.v2, new Vec2f(textureCoords.v2)),
                temp.rasterType,
                temp.texture
            );
        }

        public RPolygon getTopFlat() {
            if (sorted) {
                return getTopFlat(this);
            }
            else {
                return getTopFlat(reSort());
            }
        }

        public static RPolygon getTopFlat(RPolygon temp) {

            float v0z = 1.0f / temp.v0.z;
            float v1z = 1.0f / temp.v1.z;
            float v2z = 1.0f / temp.v2.z;

            Vec2f flatEdge = temp.flatCoeff();
            Vec3f w = temp.nlambdas(flatEdge);
            float z = 1.0f / (w.x * v0z + w.y * v1z + w.z * v2z);

            Trianglef geometry = new Trianglef(
                new Vec3f(temp.v1),
                new Vec3f(flatEdge, z),
                new Vec3f(temp.v2)
            );

            Trianglef color = new Trianglef(
                new Vec3f(temp.c1),
                new Vec3f(
                    (temp.r0 * v0z * w.x + temp.r1 * v1z * w.y + temp.r2 * v2z * w.z) * z,
                    (temp.g0 * v0z * w.x + temp.g1 * v1z * w.y + temp.g2 * v2z * w.z) * z,
                    (temp.b0 * v0z * w.x + temp.b1 * v1z * w.y + temp.b2 * v2z * w.z) * z
                ),
                new Vec3f(temp.c2)
            );

            Trianglef textureCoords = new Trianglef(
                new Vec3f(temp.t1),
                new Vec3f(
                    (temp.tu0 * v0z * w.x + temp.tu1 * v1z * w.y + temp.tu2 * v2z * w.z) * z,
                    (temp.tv0 * v0z * w.x + temp.tv1 * v1z * w.y + temp.tv2 * v2z * w.z) * z,
                    0.0f
                ),
                new Vec3f(temp.t2)
            );

            Trianglef normals = new Trianglef(
                new Vec3f(temp.n1),
                new Vec3f(
                    (temp.nx0 * v0z * w.x + temp.nx1 * v1z * w.y + temp.nx2 * v2z * w.z) * z,
                    (temp.ny0 * v0z * w.x + temp.ny1 * v1z * w.y + temp.ny2 * v2z * w.z) * z,
                    (temp.nz0 * v0z * w.x + temp.nz1 * v1z * w.y + temp.nz2 * v2z * w.z) * z
                ),
                new Vec3f(temp.n2)
            );

            return new RPolygon(
                new RVertex(geometry.v0, color.v0, normals.v0, new Vec2f(textureCoords.v0)),
                new RVertex(geometry.v1, color.v1, normals.v1, new Vec2f(textureCoords.v1)),
                new RVertex(geometry.v2, color.v2, normals.v2, new Vec2f(textureCoords.v2)),
                temp.rasterType,
                temp.texture
            );
        }

        public Vec3f normalCalc() {

            return (v1 - v0).cross(v2 - v0);
        }

        public Vec3f normalizedNormalCalc() {

            return normalCalc().unit();
        }

        public Vec3f centerCalc() {
            return new Vec3f(
                (v0.x + v1.x + v2.x) / 3.0f,
                (v0.y + v1.y + v2.y) / 3.0f,
                (v0.z + v1.z + v2.z) / 3.0f
            );
        }

        public float screenArea() {
            return edgeFunction(v0, v1, v2);
        }

        public float screenAreaCCW() {
            return edgeFunction(v0, v2, v1);
        }

        public static float edgeFunction(Vec2f a, Vec2f b, Vec2f c) {
            //return (c[0] - a[0]) * (b[1] - a[1]) - (c[1] - a[1]) * (b[0] - a[0]);
            //return (c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x);
            return (c - a).crossLenght(b - a);
        }

        public static float edgeFunction(Vec3f a, Vec3f b, Vec3f c) {
            return edgeFunction(new Vec2f(a), new Vec2f(b), new Vec2f(c));
        }

        public Vec3f lambdas(Vec2f p) {

            /*
            float w0 = edgeFunction(v1, v2, new Vec3f(p));
            float w1 = edgeFunction(v2, v0, new Vec3f(p));
            float w2 = edgeFunction(v0, v1, new Vec3f(p));
            float w0 = edgeFunction(new Vec2f(v1), new Vec2f(v2), p);
            float w1 = edgeFunction(new Vec2f(v2), new Vec2f(v0), p);
            float w2 = edgeFunction(new Vec2f(v0), new Vec2f(v1), p);*/

            Vec2f v0 = new Vec2f(this.v0);
            Vec2f v1 = new Vec2f(this.v1);
            Vec2f v2 = new Vec2f(this.v2);

            float w0 = (p - v1).crossLenght(v2 - v1);
            float w1 = (p - v2).crossLenght(v0 - v2);
            float w2 = (p - v0).crossLenght(v1 - v0);

            return new Vec3f(w0, w1, w2);
        }

        public Vec3f lambdasCCW(Vec2f p) {

            Vec2f v0 = new Vec2f(this.v0);
            Vec2f v1 = new Vec2f(this.v1);
            Vec2f v2 = new Vec2f(this.v2);

            float w0 = (p - v2).crossLenght(v1 - v2);
            float w1 = (p - v1).crossLenght(v0 - v1);
            float w2 = (p - v0).crossLenght(v2 - v0);

            return new Vec3f(w0, w1, w2);
        }

        public Vec3f nlambdas(Vec2f p) {

            float area = this.screenArea();
            float oneOverArea = 1.0f / area;
            //float w0 = edgeFunction(v1, v2, new Vec3f(p));
            //float w1 = edgeFunction(v2, v0, new Vec3f(p));
            //float w2 = edgeFunction(v0, v1, new Vec3f(p));

            Vec2f v0 = new Vec2f(this.v0);
            Vec2f v1 = new Vec2f(this.v1);
            Vec2f v2 = new Vec2f(this.v2);

            float w0 = (p - v1).crossLenght(v2 - v1);
            float w1 = (p - v2).crossLenght(v0 - v2);
            float w2 = (p - v0).crossLenght(v1 - v0);

            return new Vec3f(w0 * oneOverArea, w1 * oneOverArea, w2 * oneOverArea);
        }

        public Vec3f nlambdasCCW(Vec2f p) {

            float area = this.screenAreaCCW();
            float oneOverArea = 1.0f / area;

            //Vec3f w = lambdasCCW(p);
            Vec2f v0 = new Vec2f(this.v0);
            Vec2f v1 = new Vec2f(this.v1);
            Vec2f v2 = new Vec2f(this.v2);

            float w0 = (p - v2).crossLenght(v1 - v2);
            float w1 = (p - v1).crossLenght(v0 - v1);
            float w2 = (p - v0).crossLenght(v2 - v0);

            return new Vec3f(w0 * oneOverArea, w2 * oneOverArea, w1 * oneOverArea);
        }

        public bool test(Vec2f p) {

            Vec3f w = lambdas(p);
            float w0 = w.x;
            float w1 = w.y;
            float w2 = w.z;
            if (w0 >= 0 && w1 >= 0 && w2 >= 0) {
                return true;
            }
            else {
                return false;
            }
        }

        public static bool test(float w0, float w1, float w2) {

            if (w0 >= 0 && w1 >= 0 && w2 >= 0) {
                return true;
            }
            else {
                return false;
            }
        }

        public float minX() {
            if (x0 < x1 && x0 < x2) return x0;
            if (x1 < x0 && x1 < x2) return x1;
            return x2;
        }

        public float maxX() {
            if (x0 > x1 && x0 > x2) return x0;
            if (x1 > x0 && x1 > x2) return x1;
            return x2;
        }

        public float minY() {
            if (y0 < y1 && y0 < y2) return y0;
            if (y1 < y0 && y1 < y2) return y1;
            return y2;
        }

        public float maxY() {
            if (y0 > y1 && y0 > y2) return y0;
            if (y1 > y0 && y1 > y2) return y1;
            return y2;
        }

        public Vec3f getOriginal(int key) {
            int i = order[key];
            Vec3f result;
            switch (i) {
                case 0: {
                        result = v0;
                    }
                    break;
                case 1: {
                        result = v1;
                    }
                    break;
                case 2: {
                        result = v2;
                    }
                    break;
                default: {
                        result = v0;
                    }
                    break;
            }
            return result;
        }

        public void setOriginal(int key, Vec3f value) {
            int i = order[key];
            switch (i) {
                case 0: {
                        v0 = value;
                    }
                    break;
                case 1: {
                        v1 = value;
                    }
                    break;
                case 2: {
                        v2 = value;
                    }
                    break;
                default: {
                        v0 = value;
                    }
                    break;
            }
        }

        public Vec3f this[int key] {
            get {
                Vec3f result;
                switch (key) {
                    case 0: {
                            result = v0;
                        }
                        break;
                    case 1: {
                            result = v1;
                        }
                        break;
                    case 2: {
                            result = v2;
                        }
                        break;
                    default: {
                            result = v0;
                        }
                        break;
                }
                return result;
            }
            set {
                switch (key) {
                    case 0: {
                            v0 = value;
                        }
                        break;
                    case 1: {
                            v1 = value;
                        }
                        break;
                    case 2: {
                            v2 = value;
                        }
                        break;
                    default: {
                            v0 = value;
                        }
                        break;
                }
            }
        }

        private static int lastId = 0;
    }
}
