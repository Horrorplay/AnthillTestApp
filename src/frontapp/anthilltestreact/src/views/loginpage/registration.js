import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';

const Registration = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [passwordConfirm, setPasswordConfirm] = useState('');
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');

    const handleRegistration = async () => {
        try {
            const response = await axios.post('http://localhost:5001/api/authentication/Register', {
                email,
                password,
                passwordConfirm,
                firstName,
                lastName,
            });

            const token = response.data.token;
            localStorage.setItem('jwtToken', token);

            axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;

            console.log(response.data);
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <div>
            <h2>Registration</h2>
            <input
                type="text"
                placeholder="Email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
            />
            <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
            />
            <input
                type="password"
                placeholder="Confirm Password"
                value={passwordConfirm}
                onChange={(e) => setPasswordConfirm(e.target.value)}
            />
            <input
                type="text"
                placeholder="First Name"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
            />
            <input
                type="text"
                placeholder="Last Name"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
            />
            <button onClick={handleRegistration}>Register</button>
            <Link to="/">Go to Main Page</Link>
        </div>
    );
};

export default Registration;