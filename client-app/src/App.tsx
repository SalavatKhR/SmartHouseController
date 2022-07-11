import React from 'react';
import logo from './logo.svg';
import './App.css';
import {createBrowserHistory} from "history";
import {Route, Router, Routes} from "react-router";
import {Provider} from 'mobx-react'
import {BrowserRouter} from "react-router-dom";
import {MainPage} from "./pages/MainPage/Main";
import {Auth} from "./pages/Login/Auth/Auth";
import {Regis} from "./pages/Login/Regis/Regis";
import {ControllersPage} from "./pages/ControllersPage/ControllersPage";
import {ControllerPage} from "./pages/ControllerPage/ControllerPage";
import mainStore from "./stores/MainStore/MainStore";

const history = createBrowserHistory();

function App() {
    return (
            <Provider {...mainStore}>
                <BrowserRouter>
                    <Routes>
                        <Route path='/' element={<MainPage/>}/>
                        <Route path='/authorization' element={<Auth/>}/>
                        <Route path='/registration' element={<Regis/>}/>
                        <Route path='/controllers' element={<ControllersPage/>}/>
                        <Route path='/controllers/:id' element={<ControllerPage/>}/>
                    </Routes>
                </BrowserRouter>
            </Provider>
    );
}

export default App;
