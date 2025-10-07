import useFetch from "./functions/useFetch";
import Card from "react-bootstrap/Card";
import Container from "react-bootstrap/Container";
import './App.css';
//pass in the URL to the useFetch function if successful the data holds the data from the API
function App() {
  const { data, loading, error } = useFetch(
    "https://localhost:7199/api/StocksAPI"
  );
  if (error) {
    console.log(error);
  }

  //data that we are getting back from the API
  // {
  //   "stockId": "71f442a6-9a51-413b-a610-03d5ddcb4a10",
  //   "productName": "string",
  //   "productType": "string",
  //   "productDescription": "string",
  //   "price": "0",
  //   "quantity": 0
  // }

  return (
    <Container fluid>
      {loading && <div>Loading...</div>}
      {error && <div style={{ color: "red" }}>{error.message || String(error)}</div>}
      {data && (
        <div className='product-container'>
          {data.map((item) => (
            <Card style={{ width: "28rem", padding: "10px" }}>
              <Card.Body>
                <h2 className='bodytext-Title'> {item.productName}</h2>
                <p>Category - {item.productType}</p>
                <p>{item.productDescription}</p>
                <h4>Price: ${item.price}</h4>
              </Card.Body>
            </Card>
          ))}
        </div>
      )}
    </Container>
  );
}
export default App;
