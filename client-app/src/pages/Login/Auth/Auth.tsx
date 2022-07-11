import React from 'react';
import logo from "../../../assets/icons/logo.svg";
import {AuthContWithStore} from "../../../containers/AuthCont/AuthContWithStore";
import styles from '../Logo-style.module.sass'

export function Auth() {

    return (
        <div className={styles.auth}>
            <img src={logo} className={styles.logo} alt="logo"/>
            <AuthContWithStore/>
        </div>
    );
}
