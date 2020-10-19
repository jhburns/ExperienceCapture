/**
 * Add a cookie to the website.
 *
 * @param {string} cookieName - A key to save the cookie with.
 * @param {string} cookieValue - A value for the cookie.
 */
function createCookie(cookieName, cookieValue) {
  document.cookie = `${cookieName} = ${cookieValue}; SameSite=Strict; Path=/`;
}

/**
 * Delete a cookie from the website.
 *
 * @param {string} name - A key to be deleted.
 */
function deleteCookie(name) {
  document.cookie = `${name} = Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;`;
}

export { createCookie, deleteCookie };