﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathematicalEntities {

    public class Vec3f : IEquatable<Vec3f> {

        public float x;
        public float y;
        public float z;

        public float r { get => x; set => x = value; }
        public float g { get => y; set => y = value; }
        public float b { get => z; set => z = value; }

        public Vec3f() {
            this.x = 0.0f;
            this.y = 0.0f;
            this.z = 0.0f;
        }

        public Vec3f(Vec2f other) {
            this.x = other.x;
            this.y = other.y;
            this.z = 0.0f;
        }

        public Vec3f(Vec2f other, float z) {
            this.x = other.x;
            this.y = other.y;
            this.z = z;
        }

        public Vec3f(Vec3f other) {
            this.x = other.x;
            this.y = other.y;
            this.z = other.z;
        }

        public Vec3f(Vec4f other) {
            this.x = other.x;
            this.y = other.y;
            this.z = other.z;
        }

        public Vec3f(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vec3f(float value) {
            this.x = value;
            this.y = value;
            this.z = value;
        }

        public float dot(Vec3f other) {
            return this.x * other.x + this.y * other.y + this.z * other.z;
        }

        public Vec3f cross(Vec3f other) {
            return new Vec3f(this.y * other.z - this.z * other.y, this.z * other.x - this.x * other.z, this.x * other.y - this.y * other.x);
        }

        public float len() {
            return (float)Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
        }

        public float square() {
            return this.x * this.x + this.y * this.y + this.z * this.z;
        }

        public void normalize() {
            float lenght = len();
            if (lenght > 0.0f) {
                float invLenght = 1.0f / lenght;
                this.x *= invLenght;
                this.y *= invLenght;
                this.z *= invLenght;
            }
        }

        public Vec3f normal() {
            Vec3f res = new Vec3f(this);
            float lenght = res.len();
            if (lenght > 0.0f) {
                float invLenght = 1.0f / lenght;
                res.x *= invLenght;
                res.y *= invLenght;
                res.z *= invLenght;
            }
            return res;
        }

        public Vec3f unit() {
            Vec3f result = new Vec3f();
            result.x = this.x;
            result.y = this.y;
            result.z = this.z;
            result.normalize();
            return result;
        }

        public Vec3f slerp(Vec3f other, float a) {

            float CosAlpha = dot(other);
            float Alpha = (float)Math.Acos(CosAlpha);
            float SinAlpha = (float)Math.Sin(Alpha);
            float t1 = (float)Math.Sin((1.0f - a) * Alpha) / SinAlpha;
            float t2 = (float)Math.Sin(a * Alpha) / SinAlpha;

            return this * t1 + other * t2;
        }

        public Vec3f rotateX(float angle) {

            float Cs = (float)Math.Cos(angle);
            float Si = (float)Math.Sin(angle);
            return new Vec3f(
                this.x,
                this.y * Cs - this.z * Si,
                this.y * Si + this.z * Cs
            );
        }

        public Vec3f rotateY(float angle) {

            float Cs = (float)Math.Cos(angle);
            float Si = (float)Math.Sin(angle);
            return new Vec3f(
                this.x * Cs + this.z * Si,
                this.y,
                -this.x * Si + this.z * Cs
            );
        }

        public Vec3f rotateZ(float angle) {

            float Cs = (float)Math.Cos(angle);
            float Si = (float)Math.Sin(angle);
            return new Vec3f(
                this.x * Cs - this.y * Si,
                this.x * Si + this.z * Cs,
                this.z
            );
        }

        public Vec3f projAmt(Vec3f other) => this.proj(other.unit());

        public Vec3f perpAmt(Vec3f other) => this.perp(other.unit());

        public Vec3f proj(Vec3f n) => this.dot(n) * n;
        public Vec3f perp(Vec3f n) => this - this.proj(n);

        public Vec3f reflect(Vec3f planeNormal) => 2.0f * ((this.negate().dot(planeNormal)) * planeNormal) + this;

        public Vec3f refract(Vec3f planeNormal, float eta) {

            float dp = dot(planeNormal);
            float k = 1.0f - eta * eta * (1.0f - dp * dp);
            if (k < 0.0)
                return new Vec3f();
            else
                return eta * this - (eta * dp + (float)Math.Sqrt(k)) * planeNormal;
        }

        public Vec3f negate() => new Vec3f(-1.0f * this.x, -1.0f * this.y, -1.0f * this.z);

        public bool Equals(Vec3f other) {

            double difference_x = Math.Abs(this.x * .0001f + float.Epsilon);
            double difference_y = Math.Abs(this.y * .0001f + float.Epsilon);
            double difference_z = Math.Abs(this.z * .0001f + float.Epsilon);

            if (Math.Abs(this.x - other.x) <= difference_x && Math.Abs(this.y - other.y) <= difference_y && Math.Abs(this.z - other.z) <= difference_z) {
                return true;
            }
            else {
                return false;
            }
        }

        public float this[int i] {
            get {
                if (i == 0) return this.x;
                if (i == 1) return this.y;
                return this.z;
            }
            set {
                if (i == 0) this.x = value;
                else if (i == 1) this.y = value;
                else this.z = value;
            }
        }

        public static Vec3f operator +(Vec3f v1, Vec3f v2) => new Vec3f(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);

        public static Vec3f operator -(Vec3f v1, Vec3f v2) => new Vec3f(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);

        //public static Vec3f operator *(Vec3f v1, Vec3f v2) => new Vec3f(v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
        public static Vec3f operator *(Vec3f v1, Vec3f v2) => new Vec3f(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);

        public static Vec3f operator *(Vec3f v1, float s1) => new Vec3f(v1.x * s1, v1.y * s1, v1.z * s1);

        public static Vec3f operator *(float s1, Vec3f v1) => new Vec3f(v1.x * s1, v1.y * s1, v1.z * s1);
    }
}
