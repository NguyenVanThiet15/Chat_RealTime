// src/pages/Register.jsx
import React from "react";
import { Form, Input, Button, Alert } from "antd";
import { registerUser } from "../Features/authApi";
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";

const Register = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const { loading, error } = useSelector((state) => state.auth);

  const onFinish = async (values) => {
    debugger;
    const resultRegister = await dispatch(registerUser(values));
    if (registerUser.fulfilled.match(resultRegister)) {
      navigate("/login");
    }
  };
  return (
    <Form onFinish={onFinish} style={{ maxWidth: 400, margin: "40px auto" }}>
      {error && <Alert message={error} type="error" showIcon />}
      <Form.Item
        name="userName"
        rules={[{ required: true, message: "Vui lòng nhập tài khoản!" }]}
      >
        <Input placeholder="Tài khoản" />
      </Form.Item>
      <Form.Item
        name="email"
        rules={[
          {
            required: true,
            type: "email",
            message: "Vui lòng nhập email hợp lệ!",
          },
        ]}
      >
        <Input placeholder="Email" />
      </Form.Item>
      <Form.Item
        name="password"
        rules={[{ required: true, message: "Vui lòng nhập mật khẩu!" }]}
      >
        <Input.Password placeholder="Mật khẩu" />
      </Form.Item>
      <Form.Item>
        <Button type="primary" htmlType="submit" loading={loading} block>
          Đăng ký
        </Button>
      </Form.Item>
    </Form>
  );
};

export default Register;
