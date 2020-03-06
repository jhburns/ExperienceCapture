// TODO: refactor this to use consistent naming scheme
// And less repetition
async function postData(url = '', data = {}) {
  const response = await fetch(url, {
    method: 'POST',
    mode: 'cors', 
    cache: 'no-cache', 
    credentials: 'same-origin',
    headers: {
      'Content-Type': 'application/json'
    },
    redirect: 'follow',
    referrerPolicy: 'no-referrer',
    body: JSON.stringify(data) // body data type must match "Content-Type" header
  });
  return await response;
}

async function getData(url = '') {
  const response = await fetch(url, {
    method: 'GET',
    mode: 'cors',
    cache: 'no-cache',
    credentials: 'same-origin',
    headers: {
      'Content-Type': 'application/json'
    },
    redirect: 'follow',
    referrerPolicy: 'no-referrer',
  });
  return await response;
}

async function deleteData(url = '') {
  const response = await fetch(url, {
    method: 'DELETE',
    mode: 'cors',
    cache: 'no-cache',
    credentials: 'same-origin',
    headers: {
      'Content-Type': 'application/json'
    },
    redirect: 'follow',
    referrerPolicy: 'no-referrer',
  });
  return await response;
}

async function wait(milliseconds) {
  return new Promise(res => setTimeout(res, milliseconds));
}

async function pollGet(url ='') {
  // Expects a return to break the loop
  while (true) {
    try {
      const poll = await getData(url);

	    // Required in case session is expectantly closed
	    // Then reopened and so it has to be re-exported
	    if (poll.status === 404) {
	  	    return null;
	    }

      if (!poll.ok) {
        throw Error(poll.status);
      }

      if (poll.status === 200) {
        return poll;
      }

      await wait(3000); // 3 seconds
    } catch (err) {
      throw Error(err);
    }
  }
}

export { postData, getData, deleteData, pollGet, wait, };