/**
 * This module contains basic case formatting functions.
 */

/**
 * Return title case of given string.
 * Title Case -> Begin with upper case and others are lower case. 
 * @param {string} s *Eg*: "this Is not title Case"
 * @returns {string} *Eg*: "This Is Title Case"
 */
export function titleCase(s) {
    return s.toLowerCase()
        .split(' ')
        .map(word => word.charAt(0).toUpperCase() + word.slice(1))
        .join(' ');
}