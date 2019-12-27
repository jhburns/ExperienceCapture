function createCookie(cookieName, cookievalue) {
  document.cookie = `${cookieName} = ${cookievalue}; SameSite = Strict`;
}

export { createCookie };