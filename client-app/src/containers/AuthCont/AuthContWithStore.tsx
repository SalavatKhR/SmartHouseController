import {Input} from "../../components/ui/Input/Input";
import {Button} from '../../components/ui/Button/Button';
import React, {useEffect} from "react";
import {Question} from "../../components/ui/Question/Question";
import {useNavigate} from "react-router";
import {Loading} from "../../components/ui/Loading/Loading";
import {useStores} from "../../utils/Utils";
import {observer} from "mobx-react";

export const AuthContWithStore = observer(() => {

    const {
        authStore:
            {
                email,
                password1,
                emailError,
                password1Error,
                inputError,
                formValid,
                isRegistered,
                setFieldValue,
                validateEmail,
                validatePassword1

            }
    } = useStores();

    const navigate = useNavigate();

    useEffect(() => {
        if (email.length > 0 || password1.length > 0) {
            setFieldValue(true, formValid);

        } else {
            setFieldValue(false, formValid)
        }
    }, [email, password1])

    const onButtonClick = () => {
        if (emailError || password1Error) {
            setFieldValue(false, formValid);
            setFieldValue('Неверные пароль или логин', inputError)
        } else {
            setFieldValue(true, formValid);
            setFieldValue('', inputError)
            setFieldValue(true, isRegistered)
            setTimeout(() => {
                setFieldValue(false, isRegistered)
                navigate("/controllers")
            }, 3000, 3001);

        }
    }

    return (
        <div className="authCont">
            <Input type="email" value={email} onChange={validateEmail} placeholder="Адрес электронной почты"/>
            <Input className = "last-input" type="password" value={password1} onChange={validatePassword1} placeholder="Пароль"/>
            {(inputError) && <div className = "inputError" >{inputError}</div>}
            {isRegistered ?
                <Loading/> :
                <Button type="submit" disabled={!formValid} onClick={onButtonClick} buttonText="Вход"/>
            }
            <Question question='Еще не зарегистрированы?' href='/registration' hrefText='Регистрация'/>
        </div>
    );
})