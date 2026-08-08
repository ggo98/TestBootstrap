export default class Color {
    constructor(r, g, b, a = 1) {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    static fromString(str) {
        // matches "rgb(r, g, b)" or "rgba(r, g, b, a)"
        var match = str.match(/rgba?\(([^)]+)\)/);
        if (!match)
            throw new Error(`Not a valid rgb/rgba string: ${str}`);

        var parts = match[1].split(',').map(s => parseFloat(s.trim()));
        var [r, g, b, a = 1] = parts;
        return new Color(r, g, b, a);
    }

    toString(removeA) {
        return this.a === 1 || removeA
            ? `rgb(${this.r}, ${this.g}, ${this.b})`
            : `rgba(${this.r}, ${this.g}, ${this.b}, ${this.a})`;
    }

    invert() {
        return new Color(255 - this.r, 255 - this.g, 255 - this.b, this.a);
    }
}