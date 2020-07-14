/**
 * Add a cookie to the website.
 *
 * @param {string} cookieName - A key to save the cookie with.
 * @param {string} cookieValue - A value for the cookie.
 */
function createCookie(cookieName, cookieValue) {
  document.cookie = `${cookieName} = ${cookieValue}; SameSite = Strict; path=/`;
}

export { createCookie };