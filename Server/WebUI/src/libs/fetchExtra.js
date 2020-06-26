// TODO: check or recover from access token expiration
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

  const merged = Object.assign(base, customizations) 

  return merged
}

async function postData(url = '', data = {}) {
  const response = await fetch(url, config({
    method: 'POST',
    body: JSON.stringify(data) // body data type must match "Content-Type" header
  }));
  return await response;
}

async function getData(url = '') {
  const response = await fetch(url, config({
    method: 'GET',
  }));
  return await response;
}

async function deleteData(url = '') {
  const response = await fetch(url, config({
    method: 'DELETE',
  }));
  return await response;
}

export { postData, getData, deleteData, };