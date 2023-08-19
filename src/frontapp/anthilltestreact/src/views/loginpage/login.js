import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';

const Login = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const handleLogin = async () => {
        try {
            const response = await axios.post('http://localhost:5001/api/authentication/Login', {
                email,
                password
            });

            const token = response.data.token;
            localStorage.setItem('jwtToken', token);
           
            axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;

            console.log(response.data);
        } catch (error) {
            console.error(error.response.data.error);
        }
    };

    return (
        <div>
            <h2>Login</h2>
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
            <button onClick={handleLogin}>Login</button>
            <Link to="/">Go to Main Page</Link>
        </div>
    );
};

export default Login;