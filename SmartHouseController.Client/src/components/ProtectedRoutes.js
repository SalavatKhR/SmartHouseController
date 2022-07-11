import React from 'react';
import { Outlet } from 'react-router';
import Login from "../pages/Login";
import getCookie from "./getCookie";

function isLogged() {
    return !!getCookie();
}

const ProtectedRoutes = () => {
    return (
        isLogged() ? <Outlet/> : <Login/>
    );
};

export {
    ProtectedRoutes,
    isLogged
}