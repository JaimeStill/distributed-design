export class Strings {
    static STRING_DASHERIZE_REGEXP = /[ _]/g;
    static STRING_DECAMELIZE_REGEXP = /([a-z\d])([A-Z])/g;
    static STRING_CAMELIZE_REGEXP = /(-|_|\.|\s)+(.)?/g;
    static STRING_UNDERSCORE_REGEXP_1 = /([a-z\d])([A-Z]+)/g;
    static STRING_UNDERSCORE_REGEXP_2 = /-|\s+/g;

    static capitalize = (str: string): string =>
        str.charAt(0).toUpperCase() + str.slice(1);

    static decamelize = (str: string): string =>
        str.replace(this.STRING_DECAMELIZE_REGEXP, `$1_$2`)
           .toLowerCase();

    static dasherize = (str: string): string =>
        this.decamelize(str)
            .replace(this.STRING_DASHERIZE_REGEXP, '-');

    static camelize = (str: string): string =>
        str.replace(
            this.STRING_CAMELIZE_REGEXP,
            (_match: string, _separator: string, chr: string) =>
                chr ? chr.toUpperCase() : ''
        ).replace(/^([A-Z])/, (match: string) => match.toLowerCase());

    static classify = (str: string): string =>
        str.split('.')
           .map((part) => this.capitalize(this.camelize(part)))
           .join('');

    static underscore = (str: string): string =>
        str.replace(this.STRING_UNDERSCORE_REGEXP_1, `$1_$2`)
           .replace(this.STRING_UNDERSCORE_REGEXP_2, '_')
           .toLowerCase();

    static spacify = (str: string): string =>
        this.dasherize(str)
            .split('-')
            .map((part) => this.capitalize(part))
            .join(' ');
}
