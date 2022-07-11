import {action, makeObservable, observable} from "mobx";
import styles from '../../components/ui/Input/Input.module.sass'
import {ReactNode} from "react";

export default class AuthStore {

    email = '';
    password1 = '';
    password2 = '';
    emailError = true;
    password1Error = true;
    password2Error = true;
    inputError = '';
    formValid = false;
    isRegistered = false;

    constructor() {
        makeObservable(this, {
            email: observable,
            password1: observable,
            password2: observable,
            emailError: observable,
            password1Error: observable,
            password2Error: observable,
            inputError: observable,
            formValid: observable,
            isRegistered: observable,
            setFieldValue: action
        })
    }

    setFieldValue = (value: unknown, field: any): void => {
        // @ts-ignore
        this[field] = value;
    }

    validateEmail = (event: any) => {
        const emailValue = event.target.value
        const input = event.target

        this.setFieldValue(emailValue, this.email)
        console.log(this.email);
        const re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        if (!re.test(String(emailValue).toLowerCase())) {
            this.setFieldValue(true, this.emailError)
            input.classList.add(styles.error)
        } else {
            this.setFieldValue(false, this.emailError)
            input.classList.remove(styles.error)
        }
    }

    validatePassword1 = (event: any) => {
        const password = event.target.value
        const input = event.target
        this.setFieldValue(password, this.password1)

        if (password.length < 6) {
            this.setFieldValue(true, this.password1Error)
            input.classList.add(styles.error)
        } else {
            this.setFieldValue(false, this.password1Error)
            input.classList.remove(styles.error)
        }
    }

    validatePassword2 = (event: any) => {
        const password = event.target.value
        this.setFieldValue(password, this.password2)
        if (password !== this.password1) {
            this.setFieldValue(true, this.password2Error)
            this.setFieldValue('Пароли не совпадают', this.inputError)
        } else {
            this.setFieldValue(false, this.password2Error)
            this.setFieldValue('', this.inputError);
        }
    }
}