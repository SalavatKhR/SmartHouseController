import {NavLink} from "react-router-dom";
import React from "react";
import styles from './Question.module.sass'

export const Question = (props: any) => {
    return(
        <div className ={styles.question}>
            <p>{props.question}</p>
            <NavLink className = {styles.a} to={props.href} >
                {props.hrefText}
            </NavLink>
        </div>
    );
};