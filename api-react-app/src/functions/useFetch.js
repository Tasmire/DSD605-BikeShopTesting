import { useEffect, useState } from "react";
import axios from "axios";

//pass in the URL that you want to fetch data for
export default function useFetch(url) {
  const [data, setData] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    (async function () {
      try {
        setLoading(true);
        const response = await axios.get(url);
        setData(response.data); //have data set to data
      } catch (err) {
        setError(err); //otherwise set error
      } finally {
        setLoading(false); //set loading to false
      }
    })();
  }, [url]); //The only dependency we're going to put in the useEffect dependency array is Url because if the Url changes, we have to request new data.
  console.log(data);
  return { data, error, loading };
}

// https://dev.to/shaedrizwan/building-custom-hooks-in-react-to-fetch-data-4ig6
