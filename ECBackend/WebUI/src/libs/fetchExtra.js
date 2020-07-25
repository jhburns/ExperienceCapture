/**
 * Add more options to the base fetch configuration.
 *
 * @param {object} customizations - Optional, more configuration options.
 * @returns {object} A fetch configuration.
 */
function config(customizations = {}) {
  const base = {
    mode: 'cors',
    cache: 'no-cache',
    credentials: 'same-origin',
    headers: {
      'Content-Type': 'application/json'
    },
    redirect: 'follow',
    referrerPolicy: 'no-referrer',
  };

  const merged = Object.assign(base, customizations);

  return merged;
}

/**
 * Make a HTTP POST request.
 *
 * @param {string} url - A URL to post.
 * @param {object} data - Body of the request.
 * @returns {Response} A server response.
 */
async function postData(url, data = {}) {
  const response = await fetch(url, config({
    method: 'POST',
    body: JSON.stringify(data) // body data type must match "Content-Type" header
  }));

  const responseFinished = await response;
  return responseFinished;
}

/**
 * Make a HTTP GET request.
 *
 * @param {string} url - A URL to get.
 * @returns {Response} A server response.
 */
async function getData(url) {
  const response = await fetch(url, config({
    method: 'GET',
  }));

  const responseFinished = await response;
  return responseFinished;
}

/**
 * Make a HTTP DELETE request.
 *
 * @param {string} url - A URL to get.
 * @returns {Response} A server response.
 */
async function deleteData(url) {
  const response = await fetch(url, config({
    method: 'DELETE',
  }));

  const responseFinished = await response;

  return responseFinished;
}

export { postData, getData, deleteData, };