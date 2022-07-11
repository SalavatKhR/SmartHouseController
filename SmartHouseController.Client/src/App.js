import 'bootstrap/dist/css/bootstrap.min.css';
import Login from "./pages/Login";
import {Route, Routes} from "react-router-dom";
import {ProtectedRoutes} from "./components/ProtectedRoutes";
import Dashboard from "./pages/Dashboard";
import Registration from "./pages/Registration";

function App() {
  return (
    <div className="App">
      <Routes>
        <Route path="/Login" element={<Login/>}/>
        <Route path="/Registration" element={<Registration/>}/>
        <Route element={<ProtectedRoutes/>}>
            <Route path="/" element={<Dashboard/>}/>
        </Route>
      </Routes>
    </div>
  );
}

export default App;
