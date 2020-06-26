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

  const responseFinished = await response;
  if (responseFinished.status === 401){
    throw new Error("Posting data is unauthorized.");
  }

  return responseFinished;
}

async function getData(url = '') {
  const response = await fetch(url, config({
    method: 'GET',
  }));
  
  const responseFinished = await response;
  if (responseFinished.status === 401) {
    throw new Error("Response says unauthorized.");
  }

  return responseFinished;
}

async function deleteData(url = '') {
  const response = await fetch(url, config({
    method: 'DELETE',
  }));

  const responseFinished = await response;
  if (responseFinished.status === 401) {
    throw new Error("Response says unauthorized.");
  }

  return responseFinished;
}

export { postData, getData, deleteData, };