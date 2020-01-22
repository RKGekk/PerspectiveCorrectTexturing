using MathematicalEntities;
using ObjReader.Data.Elements;
using ObjReader.Loaders;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicRender.Engine {

    public class Renderer {

        public byte[] buf;
        public float[] zbuf;
        public int pixelWidth;
        public float pixelWidthf;
        public int stride;
        public int pixelHeight;
        public float pixelHeightf;
        public Vec2f pixelLeftTop;
        public Vec2f pixelRightBottom;

        public RObject renderObject;
        public GameTimer timer;
        public Camera camera;

        public Renderer(GameTimer timer, byte[] buf, float[] zbuf, int pixelWidth) {

            this.buf = buf;
            this.zbuf = zbuf;
            this.pixelWidth = pixelWidth;
            this.pixelWidthf = this.pixelWidth;

            this.stride = (pixelWidth * 32) / 8;
            this.pixelHeight = buf.Length / stride;
            this.pixelHeightf = this.pixelHeight;

            this.pixelLeftTop = new Vec2f(0.0f, 0.0f);
            this.pixelRightBottom = new Vec2f(this.pixelWidthf, this.pixelHeightf);

            this.camera = new Camera(new Vec3f(0.0f, 0.0f, 0.0f), new Vec3f(0.0f, 1.0f, 0.0f), -90.0f, 0.0f);

            this.timer = timer;
        }

        public void setObject(string fileName) {

            RenderObjectLoader objectLoader = new RenderObjectLoader(fileName);
            this.renderObject = objectLoader.GetRObject();
        }

        public void printPixel(int x, int y, Vec3f color) {

            byte red = (byte)(color.x * 255.0f);
            byte green = (byte)(color.y * 255.0f);
            byte blue = (byte)(color.z * 255.0f);
            byte alpha = 255;

            int pixelOffset = (x + y * pixelWidth) * 32 / 8;
            buf[pixelOffset] = blue;
            buf[pixelOffset + 1] = green;
            buf[pixelOffset + 2] = red;
            buf[pixelOffset + 3] = alpha;
        }

        public void printPixelZ(int x, int y, float z, Vec3f color) {

            byte red = (byte)(color.x * 255.0f);
            byte green = (byte)(color.y * 255.0f);
            byte blue = (byte)(color.z * 255.0f);
            byte alpha = 255;

            int offset = (x + y * pixelWidth);
            float oneOverZ = 1.0f / z;

            if (zbuf[offset] > oneOverZ) {

                zbuf[offset] = oneOverZ;

                int pixelOffset = (x + y * pixelWidth) * 32 / 8;
                buf[pixelOffset] = blue;
                buf[pixelOffset + 1] = green;
                buf[pixelOffset + 2] = red;
                buf[pixelOffset + 3] = alpha;
            }
        }

        public void fillScreen(Vec3f color) {

            for (int y = 0; y < pixelHeight; y++)
                for (int x = 0; x < pixelWidth; x++)
                    printPixel(x, y, color);
        }

        public void fillZBuff(float z) {

            for (int y = 0; y < pixelHeight; y++) {
                for (int x = 0; x < pixelWidth; x++) {
                    int offset = (x + y * pixelWidth);
                    zbuf[offset] = z;
                }
            }
        }

        public void lmoveScreen(Vec3f fillColor, int moveAmt) {

            for (int y = 0; y < pixelHeight; y++) {
                for (int x = 0; x < pixelWidth; x++) {

                    int nextPixel = x + moveAmt;
                    if (nextPixel < pixelWidth) {
                        int pixelOffset = (x + y * pixelWidth) * 32 / 8;
                        int pixelOffsetNew = (nextPixel + y * pixelWidth) * 32 / 8;
                        buf[pixelOffset] = buf[pixelOffsetNew];
                        buf[pixelOffset + 1] = buf[pixelOffsetNew + 1];
                        buf[pixelOffset + 2] = buf[pixelOffsetNew + 2];
                        buf[pixelOffset + 3] = buf[pixelOffsetNew + 3];
                    }
                    else {
                        printPixel(x, y, fillColor);
                    }
                }
            }
        }

        public void printLine(Linef lineCoords, Vec3f color) {

            int stride = (pixelWidth * 32) / 8;
            int pixelHeight = buf.Length / stride;

            int x0 = (int)lineCoords.x0;
            int y0 = (int)lineCoords.y0;
            int x1 = (int)lineCoords.x1;
            int y1 = (int)lineCoords.y1;

            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;

            int dy = Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;

            int err = (dx > dy ? dx : -dy) / 2;
            int e2;

            for (; ; ) {

                if (!(x0 >= pixelWidth || y0 >= pixelHeight || x0 < 0 || y0 < 0))
                    printPixel(x0, y0, color);

                if (x0 == x1 && y0 == y1)
                    break;

                e2 = err;

                if (e2 > -dx) {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dy) {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        public void printTriangleWireframe(Trianglef triangle, Vec3f color) {

            printLine(new Linef(triangle.v0, triangle.v1), color);
            printLine(new Linef(triangle.v1, triangle.v2), color);
            printLine(new Linef(triangle.v2, triangle.v0), color);
        }

        public void printTriangleWireframe(RPolygon polygon, Vec3f color) {

            printLine(new Linef(polygon.v0, polygon.v1), color);
            printLine(new Linef(polygon.v1, polygon.v2), color);
            printLine(new Linef(polygon.v2, polygon.v0), color);
        }

        public void printPolyFlatBottomZ(RPolygon poly) {

            Vec3f v0 = new Vec3f(poly.v0);
            Vec3f v1 = new Vec3f(poly.v1);
            Vec3f v2 = new Vec3f(poly.v2);

            bool clockWise = v2.x < v1.x ? true : false;

            if (clockWise) {
                Vec3f temp = v1;
                v1 = v2;
                v2 = temp;
            }

            float height = v2.y - v0.y;

            float dx_left = (v1.x - v0.x) / height;
            float dx_right = (v2.x - v0.x) / height;

            float xStart = v0.x;
            float xEnd = v0.x;

            int iy1;
            int iy3;
            int loop_y;

            if (v0.y < 0.0f) {

                xStart = xStart + dx_left * (-v0.y);
                xEnd = xEnd + dx_right * (-v0.y);

                v0.y = 0.0f;
                iy1 = 0;
            }
            else {

                iy1 = (int)Math.Ceiling(v0.y);

                xStart = xStart + dx_left * ((float)iy1 - v0.y);
                xEnd = xEnd + dx_right * ((float)iy1 - v0.y);
            }

            if (v2.y > pixelHeight) {

                v2.y = pixelHeight;
                iy3 = (int)v2.y - 1;
            }
            else {
                iy3 = (int)Math.Ceiling(v2.y) - 1;
            }

            float v0z = 1.0f / v0.z;
            float v1z = 1.0f / v1.z;
            float v2z = 1.0f / v2.z;
            float yTemp = v0.y;
            if (v0.x >= 0 && v0.x < pixelWidth && v1.x >= 0 && v1.x < pixelWidth && v2.x >= 0 && v2.x < pixelWidth) {

                for (loop_y = iy1; loop_y <= iy3; loop_y++) {

                    int ixStart = (int)xStart;
                    int ixEnd = (int)xEnd;
                    float xTemp = xStart;

                    for (int x = ixStart; x < ixEnd; x++) {

                        Vec2f p = new Vec2f(xTemp, loop_y);
                        Vec3f w = poly.nlambdas(p);
                        float z = 1.0f / (w.x * v0z + w.y * v1z + w.z * v2z);

                        Vec3f color = new Vec3f(
                            (poly.c0.r * v0z * w.x + poly.c1.r * v1z * w.y + poly.c2.r * v2z * w.z) * z,
                            (poly.c0.g * v0z * w.x + poly.c1.g * v1z * w.y + poly.c2.g * v2z * w.z) * z,
                            (poly.c0.b * v0z * w.x + poly.c1.b * v1z * w.y + poly.c2.b * v2z * w.z) * z
                        );

                        Vec2f UV = new Vec2f(
                            (poly.t0.x * v0z * w.x + poly.t1.x * v1z * w.y + poly.t2.x * v2z * w.z) * z,
                            (poly.t0.y * v0z * w.x + poly.t1.y * v1z * w.y + poly.t2.y * v2z * w.z) * z
                        );

                        bool alphaColor = false;
                        if (poly.rasterType == RPolygon.RasterType.Textured) {
                            Vec4f texColor = poly.texture.sampleTextureA(UV);
                            if (texColor.a == 1.0f) {
                                color.r = texColor.r;
                                color.g = texColor.g;
                                color.b = texColor.b;
                            }
                            else {
                                alphaColor = true;
                            }
                        }

                        if (!alphaColor) {
                            printPixelZ(
                                x,
                                loop_y,
                                z,
                                color
                            );
                        }

                        xTemp += 1.0f;
                    }

                    xStart += dx_left;
                    xEnd += dx_right;
                    yTemp += 1.0f;
                }
            }
            else {

                for (loop_y = iy1; loop_y <= iy3; loop_y++) {

                    float left = xStart;
                    float right = xEnd;

                    xStart += dx_left;
                    xEnd += dx_right;

                    if (left < 0) {
                        left = 0;
                        if (right < 0)
                            continue;
                    }

                    if (right > pixelWidth) {
                        right = pixelWidth;
                        if (left > pixelWidth)
                            continue;
                    }

                    int ixStart = (int)left;
                    int ixEnd = (int)right;
                    float xTemp = left;

                    for (int x = ixStart; x < ixEnd; x++) {

                        Vec2f p = new Vec2f(xTemp, loop_y);
                        Vec3f w = poly.nlambdas(p);
                        float z = 1.0f / (w.x * v0z + w.y * v1z + w.z * v2z);

                        Vec3f color = new Vec3f(
                            (poly.c0.r * v0z * w.x + poly.c1.r * v1z * w.y + poly.c2.r * v2z * w.z) * z,
                            (poly.c0.g * v0z * w.x + poly.c1.g * v1z * w.y + poly.c2.g * v2z * w.z) * z,
                            (poly.c0.b * v0z * w.x + poly.c1.b * v1z * w.y + poly.c2.b * v2z * w.z) * z
                        );

                        Vec2f UV = new Vec2f(
                            (poly.t0.x * v0z * w.x + poly.t1.x * v1z * w.y + poly.t2.x * v2z * w.z) * z,
                            (poly.t0.y * v0z * w.x + poly.t1.y * v1z * w.y + poly.t2.y * v2z * w.z) * z
                        );

                        bool alphaColor = false;
                        if (poly.rasterType == RPolygon.RasterType.Textured) {
                            Vec4f texColor = poly.texture.sampleTextureA(UV);
                            if (texColor.a == 1.0f) {
                                color.r = texColor.r;
                                color.g = texColor.g;
                                color.b = texColor.b;
                            }
                            else {
                                alphaColor = true;
                            }
                        }

                        if (!alphaColor) {
                            printPixelZ(
                                x,
                                loop_y,
                                z,
                                color
                            );
                        }

                        xTemp += 1.0f;
                    }

                    yTemp += 1.0f;
                }
            }
        }

        public void printPolyFlatTopZ(RPolygon poly) {

            Vec3f v0 = new Vec3f(poly.v0);
            Vec3f v1 = new Vec3f(poly.v1);
            Vec3f v2 = new Vec3f(poly.v2);

            bool clockWise = v1.x > v0.x ? true : false;

            if (!clockWise) {
                Vec3f temp = v1;
                v1 = v0;
                v0 = temp;
            }

            float height = v2.y - v0.y;

            float dx_left = (v2.x - v0.x) / height;
            float dx_right = (v2.x - v1.x) / height;

            float xStart = v0.x;
            float xEnd = v1.x;

            int iy1;
            int iy3;
            int loop_y;

            if (v0.y < 0.0f) {

                xStart = xStart + dx_left * (-v0.y);
                xEnd = xEnd + dx_right * (-v0.y);

                v0.y = 0.0f;
                iy1 = 0;
            }
            else {

                iy1 = (int)Math.Ceiling(v0.y);

                xStart = xStart + dx_left * ((float)iy1 - v0.y);
                xEnd = xEnd + dx_right * ((float)iy1 - v0.y);
            }

            if (v2.y > pixelHeight) {

                v2.y = pixelHeight;
                iy3 = (int)v2.y - 1;
            }
            else {
                iy3 = (int)Math.Ceiling(v2.y) - 1;
            }

            float v0z = 1.0f / v0.z;
            float v1z = 1.0f / v1.z;
            float v2z = 1.0f / v2.z;
            float yTemp = v0.y;
            if (v0.x >= 0 && v0.x < pixelWidth && v1.x >= 0 && v1.x < pixelWidth && v2.x >= 0 && v2.x < pixelWidth) {

                for (loop_y = iy1; loop_y <= iy3; loop_y++) {

                    int ixStart = (int)xStart;
                    int ixEnd = (int)xEnd;
                    float xTemp = xStart;

                    for (int x = ixStart; x < ixEnd; x++) {

                        Vec2f p = new Vec2f(xTemp, loop_y);
                        Vec3f w = poly.nlambdas(p);
                        float z = 1.0f / (w.x * v0z + w.y * v1z + w.z * v2z);

                        Vec3f color = new Vec3f(
                            (poly.c0.r * v0z * w.x + poly.c1.r * v1z * w.y + poly.c2.r * v2z * w.z) * z,
                            (poly.c0.g * v0z * w.x + poly.c1.g * v1z * w.y + poly.c2.g * v2z * w.z) * z,
                            (poly.c0.b * v0z * w.x + poly.c1.b * v1z * w.y + poly.c2.b * v2z * w.z) * z
                        );

                        Vec2f UV = new Vec2f(
                            (poly.t0.x * v0z * w.x + poly.t1.x * v1z * w.y + poly.t2.x * v2z * w.z) * z,
                            (poly.t0.y * v0z * w.x + poly.t1.y * v1z * w.y + poly.t2.y * v2z * w.z) * z
                        );

                        bool alphaColor = false;
                        if (poly.rasterType == RPolygon.RasterType.Textured) {
                            Vec4f texColor = poly.texture.sampleTextureA(UV);
                            if (texColor.a == 1.0f) {
                                color.x = texColor.r;
                                color.y = texColor.g;
                                color.z = texColor.b;
                            }
                            else {
                                alphaColor = true;
                            }
                        }

                        if (!alphaColor) {
                            printPixelZ(
                                x,
                                loop_y,
                                z,
                                color
                            );
                        }

                        xTemp += 1.0f;
                    }

                    xStart += dx_left;
                    xEnd += dx_right;
                    yTemp += 1.0f;
                }
            }
            else {

                for (loop_y = iy1; loop_y <= iy3; loop_y++) {

                    float left = xStart;
                    float right = xEnd;

                    xStart += dx_left;
                    xEnd += dx_right;

                    if (left < 0) {
                        left = 0;
                        if (right < 0)
                            continue;
                    }

                    if (right > pixelWidth) {
                        right = pixelWidth;
                        if (left > pixelWidth)
                            continue;
                    }

                    int ixStart = (int)left;
                    int ixEnd = (int)right;
                    float xTemp = left;

                    for (int x = ixStart; x < ixEnd; x++) {

                        Vec2f p = new Vec2f(xTemp, loop_y);
                        Vec3f w = poly.nlambdas(p);
                        float z = 1.0f / (w.x * v0z + w.y * v1z + w.z * v2z);

                        Vec3f color = new Vec3f(
                            (poly.c0.r * v0z * w.x + poly.c1.r * v1z * w.y + poly.c2.r * v2z * w.z) * z,
                            (poly.c0.g * v0z * w.x + poly.c1.g * v1z * w.y + poly.c2.g * v2z * w.z) * z,
                            (poly.c0.b * v0z * w.x + poly.c1.b * v1z * w.y + poly.c2.b * v2z * w.z) * z
                        );

                        Vec2f UV = new Vec2f(
                            (poly.t0.x * v0z * w.x + poly.t1.x * v1z * w.y + poly.t2.x * v2z * w.z) * z,
                            (poly.t0.y * v0z * w.x + poly.t1.y * v1z * w.y + poly.t2.y * v2z * w.z) * z
                        );

                        bool alphaColor = false;
                        if (poly.rasterType == RPolygon.RasterType.Textured) {
                            Vec4f texColor = poly.texture.sampleTextureA(UV);
                            if (texColor.a == 1.0f) {
                                color.x = texColor.r;
                                color.y = texColor.g;
                                color.z = texColor.b;
                            }
                            else {
                                alphaColor = true;
                            }
                        }
                        if (!alphaColor) {
                            printPixelZ(
                                x,
                                loop_y,
                                z,
                                color
                            );
                        }

                        xTemp += 1.0f;
                    }

                    yTemp += 1.0f;
                }
            }
        }

        public void printPolyZ(RPolygon pIn) {

            if (pIn.isEpsilonGeometry())
                return;

            RPolygon polygon = pIn.reSort();

            if (polygon.isOffScreen(this.pixelLeftTop, this.pixelRightBottom))
                return;

            if (polygon.isFlatTop()) {
                printPolyFlatTopZ(polygon);
            }
            else {
                if (polygon.isFlatBottom()) {
                    printPolyFlatBottomZ(polygon);
                }
                else {

                    RPolygon p1 = polygon.getBottomFlat();
                    printPolyFlatBottomZ(p1);

                    RPolygon p2 = polygon.getTopFlat();
                    printPolyFlatTopZ(p2);
                }
            }
        }

        public void renderPolySolidColorZ(bool isWireframe) {

            renderObject.model = new Mat4f(
                new Vec4f(1.0f, 0.0f, 0.0f, 0.0f),
                new Vec4f(0.0f, 1.0f, 0.0f, 0.0f),
                new Vec4f(0.0f, 0.0f, 1.0f, 0.0f),
                new Vec4f(0.0f, 0.0f, 0.0f, 1.0f)
            );
            renderObject.calculateModel();

            renderObject.world = new Mat4f(
                new Vec4f(1.0f, 0.0f, 0.0f, 0.0f),
                new Vec4f(0.0f, 1.0f, 0.0f, 0.0f),
                new Vec4f(0.0f, 0.0f, 1.0f, 0.0f),
                new Vec4f(0.0f, 0.0f, 6.0f, 1.0f)
            );
            renderObject.calculateWorld();

            renderObject.view = camera.GetViewMatrix();
            renderObject.calculateView(false);

            renderObject.proj = camera.GetProjMatrix();
            renderObject.calculateProj();

            renderObject.calculateRaster(pixelWidthf, pixelHeightf, true);

            foreach (RPolygon polygon in renderObject.polygons_raster) {
                printPolyZ(polygon);
            }

            if (isWireframe) {
                foreach (RPolygon polygon in renderObject.polygons_raster) {
                    printTriangleWireframe(polygon, new Vec3f(1.0f, 0.0f, 0.0f));
                }
            }

            string ffds = "sfv";
        }
    }
}
